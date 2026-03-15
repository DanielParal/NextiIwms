
Translation

Name - pageNotFound
Scope - nexti-pageNotFound
Key - Scope + Name
Value - Stranka nenalezena
Language - cz, en



Name - pageNotFound
Scope - nexti-pageNotFound
Key - nexti-pageNotFound-pageNotFound
Value - Stranka nenalezena
Language - cz



<!-- {
    scope: nexti-pageNotFound,
    defaults: [
        { pageNotFound, 'Stranka nenalezena' },
        { dalsiPriklad, 'Dalsi priklad' }
    ],
    meta: [
        {MetoInfo1, 'Meta info 1'}
    ]
}


nexti-pageNotFound-pageNotFound cz 'Stranka nenalezena'
nexti-pageNotFound-dalsiPriklad cz 'Dalsi priklad'


Masstransit event


nexti-pageNotFound-pageNotFound en 'Stranka nenalezena'
nexti-pageNotFound-dalsiPriklad en 'Dalsi priklad'

nexti-pageNotFound-pageNotFound fr 'Stranka nenalezena'
nexti-pageNotFound-dalsiPriklad fr 'Dalsi priklad' -->



Flow 1:
FE -> get translations/scope - /traslations/nexti-pageNotCound (language - cz z headeru)

BE returns:
[
    { pageNotFound, 'Stranka nenalezena' },
    { dalsiPriklad, 'Dalsi priklad' }
]

FE loops through the translations -> if not found - send create translation:

{
    scope: nexti-pageNotFound,
    name: pageNotFound,
    value: 'Stranka nenalezena'
}

FE does not wait for response and display the default value

BE creates translation for cz and publishes masstransit message

BE consumer creates the rest of translations for the rest of languages which are enabled for the tenant




Languages preference:

Local storage -> Browser language -> cz default
BE gets the value from header -> if not found - returns server language (cz)


Create:
Export Import for Translations