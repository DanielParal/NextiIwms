# Jak nasadit aplikaci na novem windows serveru:

## 1. Potrebny software, ktery je potreba nainstalovat:

### 1.1. Nainstalovat Azure cli
- https://learn.microsoft.com/en-us/cli/azure/install-azure-cli-windows?tabs=azure-cli

### 1.2. Nainstalovat Git
- https://git-scm.com/downloads/win

### 1.3. Nainstalovat Docker desktop
- https://docs.docker.com/desktop/setup/install/windows-install/

## 2. Vytvorit potrebne uzivatele/service-principals (Tento krok delat jenom pouze pokud jeste neexistuji)

### 2.1. Uzivatel pro stahovani zmen z gitu
- Vytvorime uzivatele v Azure DevOpsech pro danou organizace s Basic/readonly pristupem
- Vytvorime PAT (personal access token) pro uzivatele v teto sekci (https://dev.azure.com/NextiCZ/_usersSettings/tokens). Tokenu nastavime pouze READ prava pro repositare

### 2.2. Vytvorime service principal s readonly pristupem do Azure container registries pro stahovani docker imagu a s readonly pristupem do azure keyvaultu
- Jdeme do azuru do 'App registrations' (https://portal.azure.com/#view/Microsoft_AAD_RegisteredApps/ApplicationsListBlade) a zaregistrujeme novou aplikaci
- Poznamka: muzeme nechat nastaveni 'Accounts in this organizational directory only'
- Jdeme do detailu nove vytvorene aplikace a najdeme sekci 'Certificates & secrets'
- Vytvorime novy secret - zkopirujeme si heslo a ulozime si ho na bezpecne heslo
- Jdeme do sekce 'Container registries' (https://portal.azure.com/#browse/Microsoft.ContainerRegistry%2Fregistries) -> vybereme si nase registry -> 
jdeme do sekce 'Access control (IAM)' -> Najdeme sekci 'View access to this resource' -> Nahore dame '+ Add' -> Pridame roli 'acrpull' pro nas nove vytvoreny service principal
- Potom jdeme do azure key vaultu -> do sekce 'Access control (IAM)' -> pridame danemu service principalu roli: 'Key Vault Secrets User'

## 3. Nastaveni na serveru

### 3.1. Prihlasime nove vytvoreny service principal na serveru
- Po uspesnem vytvoreni service principalu (bod 2.2.) a uspesnem nainstalovani azure cli (bod 1.1.)
- Otevreme si prikazovy radek (cmd command) -> rozbehneme 2 scripty:
- az login --service-principal --username <clientid> --password <password> --tenant <tenantid>
- az acr login --name jipocariwms
- clientid, passord, tenandid najdeme v detailu naseho vytvoreneho service principalu (https://portal.azure.com/#view/Microsoft_AAD_RegisteredApps/ApplicationsListBlade)
- Po tomto kroku budeme schopni stahovat imagu na server z azure container registries

### 3.2. Naklonujeme si git repositar s docker compose projektem
- Jdeme do repositare, kde chceme soubory pro rozbehnuti aplikace (docker-compose.yaml, proxy a dalsi)
- Rozbehneme tam script:
- git clone https://<username>:<generated-personal-access-token>@dev.azure.com/NextiCZ/IWMS/_git/IWMS-deployment
- <username> muze byt "readonly-iwms" (bez @nexti.cz) a <generated-personal-access-token> je token, ktery jsme vygenerovali v kroku 2.1.

### 3.3. Nakopirujeme SSL certifikaty do slozky ssl v rootu projektu
- Certifikaty nam poskytuje nas zakaznik (v pripade IWMS se jedna o Jipocar)
- S nejvetsi pravdepodobnosti nam poskytnou 3 soubory: privateKey, server certifikat a intermediate certifikat
- Musime si vytvorit vlastni .pem soubory a musi se nazyvat: privkey.pem a fullchain.pem:
- Nakopirujeme privateKey souboru do privkey.pem a tento soubor vlozime do ssl slozky v rootu projektu
- Potom nakopirejeme server certifikat a intermediate certifikat (MUSIME dodrzet poradi - prvne nakopirovat server certifikat a pak az intermediate certifikat) do souboru fullchain.pem a ten vlozime do ssl slozky v rootu projektu

### 3.4. Vytvorime environment variables pro azure
- Deployment CLI tool pozuivat Azure key vault api pro stazeni secretu a na to potrebuje credentials v environment variables
- Do environment variables pridame tyto 3 promenne: AZURE_CLIENT_ID, AZURE_CLIENT_SECRET, AZURE_TENANT_ID
- Vsechny 3 hodnoty muzeme najit v Azuru v 'App registrations', kde jsme vytvorili nas service principal

### 3.5. Vytvorime konfiguracni soubor pro definici prostredi
- Jdeme do slozky deployment v rootu projektu a vytvorime novy soubor s nazvem config-environment-staging/production (podle prostredi)
- Tento soubor slouzi pro CLI tool, aby rozpoznal, v jakem je prostredi

### 3.6. Vytvorime appsettings.json konfiguracni soubor pro CLI tool
- Tento soubor se musi nachazet u CLI toolu (v deployment folderu)
- appsettings.json pro development najdete v git repositari, takze ho muzete zkopirovat
- Vsechna nastaveni by mela zustat jak jsou, az na external tools, ktere zalezi, kde jsou naistalovane dane programy. Pro zjisteni, kde jsou nainstalovany muzete vyuzit commandy:
- where git, where az, where docker-compose
- Potom je potreba nahradit 'FileConfiguration__RootDirectoryPath' soucasnou cestou do root repositare, kde se nachazi docker-compose.yaml file

### 3.7. Spustime aplikaci
- Spustime Iwms-deployment-cli.exe soubor, ktery je v deployment folderu

#### 3.7.1. Problem s Proxy image
- Pokud nastane chyba s proxy imagem, tak je to nejspis zpusobene tzdata (pro timezony). Docasne reseni je, ze zakomentuje radky:

```
# Install tzdata and set timezone
RUN apt-get update && apt-get install -y tzdata

# Set default timezone if not set
ENV TZ=Europe/Prague
```

- Po spusteni image muzeme tyto radky do image znovu pridat.

### 3.8. Nastaveni firewallu
- Musime nastavit firewally, abychom povolili porty. Jipocar ma nektere porty povolene z internetu a nektere jenom v ramci VPN.

#### 3.8.1. Porty, ktere jsou povoleny v Jipocaru z internetu
- Soucasne Inbound Rule nazyvame: 1_AllowFromInternet
- Povolujeme porty 80 a 443 z internetu

#### 3.8.2. Porty, ktere jsou povoleny v Jipocaru z VPN
- Soucasne Inbound Rule nazyvame: 2_AllowFromVPN
- Povolujeme porty 1433, 5432, 5672, 15672
- Jedna se o porty na MsSQL databazi, Postgres databazi a RabbitMQ

### 3.9. Nastaveni hosts filu
- Musime pridat tyto hodnoty do hosts filu:

```
# Added to works projects DNS localy
192.168.13.18 staging-iwms-api.jipocar.cz
192.168.13.18 staging-iwms.jipocar.cz
192.168.13.18 staging-iwms-vhla.jipocar.cz
192.168.13.18 staging-iwms-sign.jipocar.cz
192.168.13.18 info-test.jipocar.cz
192.168.13.18 infoapi-test.jipocar.cz
```

- ip adresa a url adresy se meni na zaklade prostredi

4. Click Ok
5. If there is a problem with user you need to go "compmgmt.msc" and create new user and add him to the administrators group

## 4. CLI deployment tool
- Po spusteni exe souboru mame 2 moznosti: update nebo deploy
- Update nam aktualizuje verze imagu z azure container registries a ulozi je lokalne do nazvu souboru v deployment souboru
- Deploy provede vsechny nezbytne kroky k nasazeni docker-compose.yaml filu (verze bere lokalni z nazvu souboru)

### 4.1. Jak vytvorit novou verzi CLI toolu
- Pokud udelame nejake zmeny v projektu iwms-deployment-cli, tak budeme muset Releasnout novou verzi ->
- Novou verzi releasneme s commandem: 
```
dotnet publish -c Release -r win-x64 --self-contained true
```
- Najdeme slozku, kam se nam releasnula nova verze - s nejvetsi pravdepodobnosti to bude 'bin/Release/net9.0/win-x64/publish' ->
- A prekopirujeme exe file do slozky 'deployments' v rootu projektu
- Pokud je potreba, udelame zmeny v appsettings.json filu

## 5. Databaze
- V soucasne dobe (2025-1-30) pouzivame 3 databaze v MsSQL - iwms, info-jipocar a POR (KVADOS)
- INFO: ve stagingu se dataze jmenuji ims-staging, info-jipocar-test, POR_test
- iwms a info-jipocar sedi na nasech serverech v docker composu
- POR si hostuji na svych serverech a managuji si databazi sami

### 5.1. Jak si vygenerovat script s tabulky a daty
- Tento krok slouzi k tomu, kdyz potrebujeme restornout databazi, ale nemame pristup k databazovemu serveru, takze se nemuzeme dostat k backupum
- Musime si teda vytvorit .sql script, ktery bude obsahovat vsechna schemata a data
- v MSSQL Management studio provedeme tyto kroky:
  1. Pravym tlacitkem klikneme na databazi -> Tasks -> Generate Scripts
  2. Dame Next -> Next -> V casti 'Set Scripting Options' klikneme na Advanced tlacitko a vybereme: 'Types of data to script': 'Schema and data'
  3. Pak vybereme 'Save as script file' a vybereme lokaci
  4. Pak uz dame pouze finish a mame sql script

### 5.2. Jak vytvorit noveho uzivatele na databazi
- Bud budeme vytvaret uzivatele pro uplne novou databazi nebo pokud restorneme databazi pomoci .mdf a .ldf filu, tak budeme muset fixnout stavajici uzivatele

#### 5.2.1. Vytvarime uzivatele pro uplne novou databazi
- Nejprve vytvorime databazi, nahrajeme schemata, nahrajeme data a potom rozjedete tento script
- Scipt nejprve vytvori jeden spolecny Login s heslem, ktery mate v connection stringach (ten musite upravit) -> a pote vytvori uzivatele s db_owner role pro kazdou databazi

```sql

CREATE LOGIN [Nexticz]  
WITH PASSWORD = 'YourPassword', -- Replace with your strong password 
     CHECK_POLICY = ON;  -- Enforces password complexity rules 

GO 

-------------------------
-- iwms database
------------------------- 
USE [iwms] 
GO 

-- Create a database user associated with the login 
CREATE USER [Nexticz] FOR LOGIN [Nexticz] WITH DEFAULT_SCHEMA = [dbo]; 
GO 

-- Grant db_owner role to the user 
ALTER ROLE [db_owner] ADD MEMBER [Nexticz]; 
GO

-------------------------
-- info-jipocar database
-------------------------
USE [info-jipocar] 
GO 

-- Create a database user associated with the login 
CREATE USER [Nexticz] FOR LOGIN [Nexticz] WITH DEFAULT_SCHEMA = [dbo];
GO 

-- Grant db_owner role to the user 
ALTER ROLE [db_owner] ADD MEMBER [Nexticz];
GO

```

#### 5.2.2. Vytvorit uzivatele pro zkopirovanou databazi
- Pokud zkopirejeme databazi pomoci .mdf a .ldf souboru, tak tam s nejvetsi pravdepodobnosti zustane uzivatel, ale login nebude vytvoren na serveru
- Takze nejprve budeme muset vytvorit Login s nasim heslem a pak ho pridat jednotlivym uzivatelum v databazich

```sql

-- Replace with your strong password 
CREATE LOGIN [Nexticz] WITH PASSWORD = 'YourPassword'; 
GO 

-------------------------
-- iwms database
------------------------- 
USE [iwms] 
GO 

ALTER USER [Nexticz] WITH LOGIN = [Nexticz]; 
GO 

-------------------------
-- info-jipocar database
-------------------------
USE [info-jipocar] 
GO 

ALTER USER [Nexticz] WITH LOGIN = [Nexticz]; 
GO 

```

### 5.3. Jak si zkopirovat zalohu z naseho serveru
- Jipocar dela backup serveru pravidelne a tim backupuje i nase databaze, ktere lezi na serveru
- Jak si restornout backup:
  1. Vsechny databaze se nachazeji v ramci projektu ve slozce: 'data/mssql/data'
  2. Z teto slozky si najdeme databazi, kterou chceme restornout a zkopirujeme si 2 soubory: .mdf a .ldf
  3. Tyto soubory si nakopirejeme do sve instance mssql ve svem docker composu
  4. Potom jdeme do MsSQL management studia -> pravym klikneme na Databases -> Attach -> A vybereme databazi, kterou jsme si tam nakopirovali

## 6. Monitoring tool
- Tento nastroj slouzi k monitorovani docker imagu na serveru
- Kazdou minutu kontroluje, ze bezi vsechny containery, ktere jsou nastavene v appsettings.json. Pokud vsechny nebezi, posle critical error do logu do cloudu
- V azure Application Insights je nastavene pravidlo, ktere kazdych 5 minut kontroluje logy a hleda zpravu, ze vsechny kontejnery na serveru bezi -> pokud nic nenajde, posle email
- Tato aplikace musi neustale bezet na serveru -> pokud nepobezi, tak monitorovaci nastroj bude neustale posilat emaily

### 6.1. Jak vytvorit novou verzi monitorovaciho toolu
- Pokud udelame nejake zmeny v projektu iwms-monitoring-app, tak budeme muset Releasnout novou verzi ->
- Novou verzi releasneme s commandem:
```
dotnet publish -c Release -r win-x64
```
- Najdeme slozku, kam se nam releasnula nova verze - s nejvetsi pravdepodobnosti to bude 'bin/Release/net9.0/win-x64/publish' ->
- A prekopirujeme exe file do slozky 'monitoring' v rootu projektu
- Pokud je potreba, udelame zmeny v appsettings.json filu - v produkci musime nastavit spravny insights key

### 6.2. Azure alert rules
- Na azure muzete najit 'Alert Rules' zde:
- Staging https://portal.azure.com/#@ngas.cz/resource/subscriptions/4126fe27-ef8c-4c4e-a0f0-ef1df9874a4f/resourceGroups/jipocar-iwms/providers/microsoft.insights/scheduledqueryrules/staging-alert-docker-containers-monitoring/overview
- Production https://portal.azure.com/#@ngas.cz/resource/subscriptions/4126fe27-ef8c-4c4e-a0f0-ef1df9874a4f/resourceGroups/jipocar-iwms/providers/microsoft.insights/scheduledqueryrules/prod-alert-docker-containers-monitoring/overview