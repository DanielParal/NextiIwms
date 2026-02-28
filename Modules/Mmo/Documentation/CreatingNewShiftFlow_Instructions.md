# Shift Creation Logic – AI Code Generation Instructions

## Purpose

This document describes the exact business logic used to decide **when to create a new shift** in the system.
The logic is implemented in .NET and is driven by the current time, the last known shift, and configurable thresholds.

An AI generating code from this document **must follow the decision flow exactly** and must not simplify or reorder steps.

---

## High-Level Architecture

### Components

1. **ShiftCreationOrchestrator**
    - Responsible for fetching required data.
    - Delegates decision-making to the decider.

2. **ShiftCreationDecider**
    - Contains all logic to determine whether a new shift should be created.
    - Decides between:
        - Creating no shift
        - Creating the current shift
        - Creating the next shift

---

## Definitions

- **Current Shift**  
  The shift whose time range includes `NOW`.

- **Next Shift**  
  The shift that starts immediately after the current shift.

- **End Threshold**  
  A time window (in hours) before a shift ends during which the system is allowed to create the next shift.

---

## Step-by-Step Decision Logic

### 1. Retrieve Required Data

1. Fetch `LastShift`
2. Fetch `ShiftSettings`

---

### 2. Handle First Shift Case

- If `LastShift` is `null`:
    - This only occurs when the system has **no shifts yet**.
    - **Create a new shift immediately**
    - **Stop further processing**

---

### 3. Check If Last Shift Has Ended

- If `LastShift.EndDate > NOW`:
    - Do **not** create a new shift
    - **Stop further processing**

---

### 4. Determine If We Are Within End Threshold

Calculate:


- If `HoursDifference > HoursBeforeNextShiftShouldBeCreatedConstant`:
    - Do **not** create a new shift
    - **Stop further processing**

- If `HoursDifference <= HoursBeforeNextShiftShouldBeCreatedConstant`:
    - Continue evaluation

---

### 5. Check If a Current Shift Exists

#### 5.1 If Current Shift **Exists and Is Active**

- Check if current shift is within end threshold:
    - If **YES**:
        - Fetch **Next Shift**
        - Create **Next Shift**
    - If **NO**:
        - Fetch **Current Shift**
        - Create **New Shift**

---

#### 5.2 If No Current Shift Exists

- Check if within end threshold:
    - If **YES**:
        - Create **New Shift**
    - If **NO**:
        - Do **not** create a shift

---

## Example Scenario

### Inputs

- `CurrentShift.EndDate = 4:00 PM`
- `NOW = 2:00 PM`
- `HoursBeforeNextShiftShouldBeCreatedConstant = 3 hours`

### Calculation


### Decision

- `2 <= 3` → Within end threshold
- The system **creates the next shift**
- The current shift still ends at 4:00 PM
- The next shift starts at 4:00 PM (even though it is currently 2:00 PM)

---

## Rules & Constraints

- **Never create overlapping shifts**
- **Never create multiple shifts in a single execution**
- All date comparisons must use the same time zone
- The decider must be deterministic (same inputs → same outcome)

---

## Expected Output from AI Code Generation

The generated code should:

- Be written in **.NET**
- Separate orchestration from decision logic
- Use clear method boundaries for:
    - Threshold calculation
    - Current shift detection
    - Shift creation
- Be easily unit-testable
- Match the decision flow described above exactly

---

## Non-Goals

- This document does **not** define persistence logic
- This document does **not** define database schema
- This document does **not** define scheduling or background job execution

---
