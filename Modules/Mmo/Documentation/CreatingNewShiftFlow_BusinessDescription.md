# Shift Creation – Simple Description

> ⚠️ **Important notice**  
> This description was automatically generated from a system diagram using AI.  
> It reflects the diagram logic exactly and **must not be manually modified**.  
> Any required changes must be done in the source diagram and regenerated.

---

## Simple English – How New Shifts Are Created

The system checks whether it needs to create a new shift based on the current time and existing shifts.

1. **If there are no shifts yet**
    - The system creates the first shift immediately.

2. **If the last shift has not ended yet**
    - The system does nothing.
    - No new shift is created.

3. **If the last shift has already ended**
    - The system checks how close the current time is to the end of the shift.

4. **If the current time is close enough to the shift end**  
   (within a predefined number of hours)
    - The system is allowed to create a new shift.

5. **If a current (active) shift exists**
    - And it is close to ending:
        - The system creates the **next shift** in advance.
    - If it is not close to ending:
        - The system does nothing.

6. **If no current shift exists**
    - And the system is within the allowed time window:
        - The system creates a new shift.
    - Otherwise:
        - The system does nothing.

The system never creates overlapping shifts and never creates more than one shift at a time.

---

## Česky – Jednoduchý popis vytváření směn

Systém kontroluje, zda je potřeba vytvořit novou směnu, na základě aktuálního času a existujících směn.

1. **Pokud v systému zatím neexistuje žádná směna**
    - Systém okamžitě vytvoří první směnu.

2. **Pokud poslední směna ještě neskončila**
    - Systém nic nedělá.
    - Nová směna se nevytvoří.

3. **Pokud poslední směna již skončila**
    - Systém zkontroluje, jak blízko je aktuální čas ke konci směny.

4. **Pokud je aktuální čas dostatečně blízko ke konci směny**  
   (v rámci předem definovaného počtu hodin)
    - Systém může vytvořit novou směnu.

5. **Pokud existuje aktuální (aktivní) směna**
    - A blíží se ke konci:
        - Systém vytvoří **následující směnu** dopředu.
    - Pokud se ke konci neblíží:
        - Systém nic nedělá.

6. **Pokud aktuální směna neexistuje**
    - A systém je v povoleném časovém okně:
        - Systém vytvoří novou směnu.
    - Jinak:
        - Systém nic nedělá.

Systém nikdy nevytváří překrývající se směny a nikdy nevytváří více než jednu směnu najednou.
