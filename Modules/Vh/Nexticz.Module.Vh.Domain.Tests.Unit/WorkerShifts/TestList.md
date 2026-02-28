# WorkerShifts Domain specifications

- WorkerShifts data is available with 10 minutes delay

---

# WorkerShift

## Create WorkerShift

- [x] Created WorkerShift should throw ArgumentNullException if Activity is null
- [x] Created WorkerShift must have exactly one Activity
- [x] Created WorkerShift starts at same time as first Activity
- [x] Created WorkerShift must have same WorkerCode as first Activity

## Close WorkerShift

- [x] WorkerShift property Approved is set to false
- [x] Ended WorkerShift must ends at the inserted End
- [x] If last Activity has not CutOff => last Activity End is set to WorkerShift End
- [x] If last Activity has CutOff and long enough => last Activity End is set to WorkerShift End
- [x] If last Activity has CutOff and long not enough => last Activity End as Start plus CutOff, insert between last Activity and WorkerShift End Activity with UNKNOWN type
- [x] End WorkerShift over overlapping Activity
- [x] WorkerShift must have data consistency without empty spaces, check cutOffs
- [x] Current WorkerShift can not contain trimmed Activities
- [x] Result of Close WorkerShift must return List of Trimmed off Activities

## Approve WorkerShift

- [x] WorkerShift Approved must be set to false before Approval
- [x] Approved WorkerShift must not have any UNKNOWN Activity
- [x] WorkerShift must have End
- [x] WorkerShift property Approved is set to true after approval

## DisApprove WorkerShift

- [x] WorkerShift property Approved must be set to true before DisApproval
- [x] WorkerShift property Approved is set to false after DisApproval

---

# WorkerShiftActivity

## Add Activity

- [x] WorkerShift property Approved is set to false
- [x] Can add Activity to WorkerShift
- [x] Can not add Activity, which starts after WorkerShift End
- [x] Can not add Activity with different WorkerCode than WorkerShift WorkerCode
- [x] WorkerShift must have data consistency without empty spaces, check cutOffs

### Add Activity before WorkerShift start

- [x] WorkerShift Start must be updated to Activity Start

#### Add with same ActivityCode

- [x] If added Activity has not CutOff => first Activity Start set to new Activity Start, first Activity ActivitiesCount increase +1
- [x] If added Activity has CutOff and long enough => first Activity Start set to new Activity Start, first Activity ActivitiesCount increase +1
- [x] If added Activity has CutOff and long not enough => add new Activity with End as Start plus CutOff, insert between new Activity and first Activity, Activity with UNKNOWN type

#### Add with different ActivityCode

- [x] If added Activity has not CutOff => add new Activity with End as first Activity Start
- [x] If added Activity has CutOff and long enough => add new Activity with End as first Activity Start
- [x] If added Activity has CutOff and long not enough => add new Activity with End as Start plus CutOff, insert between new Activity and first Activity, Activity with UNKNOWN type

### Add overlapping Activity

#### Add with same ActivityCode

- [x] Do nothing

#### Add with different ActivityCode and next Activity exist and is different

- [x] If overlapping Activity has not CutOff => overlapped Activity End as new Activity Start, add new Activity with End as next Activity Start
- [x] If overlapping Activity has CutOff and long enough => overlapped Activity End as new Activity Start, add new Activity with End as next Activity Start
- [x] If overlapping Activity has CutOff and long not enough => overlapped Activity End as new Activity Start, add new Activity with End as Start plus CutOff, insert between new Activity and next Activity, Activity with UNKNOWN type

#### Add with different ActivityCode and next Activity exist and has same code

- [x] If overlapping Activity has not CutOff => overlapped Activity End as new Activity Start, next Activity Start as new Activity Start, next Activity ActivitiesCount increase +1
- [x] If overlapping Activity has CutOff and long enough => overlapped Activity End as new Activity Start, next Activity Start as new Activity Start, next Activity ActivitiesCount increase +1
- [x] If overlapping Activity has CutOff and long not enough => overlapped Activity End as new Activity Start, add new Activity with End as Start plus CutOff, insert between new Activity and next Activity, Activity with UNKNOWN type

#### Add with different ActivityCode and next Activity not exist and WorkerShift End exist

- [x] If overlapping Activity has not CutOff => overlapped Activity End as new Activity Start, add new Activity with End as WorkShift End
- [x] If overlapping Activity has CutOff and long enough => overlapped Activity End as new Activity Start, add new Activity with End as WorkShift End
- [x] If overlapping Activity has CutOff and long not enough => overlapped Activity End as new Activity Start, add new Activity with End as Start plus CutOff, insert between new Activity and WorkShift End, Activity with UNKNOWN type

### Add Activity to end

#### Add with same ActivityCode

- [x] Last Activity ActivitiesCount increase +1
- [x] If last Activity has CutOff and long enough => last Activity ActivitiesCount increase +1
- [x] If last Activity has CutOff and long not enough => last Activity ActivitiesCount increase +1

#### Add with different ActivityCode

- [x] If last Activity has not CutOff => last Activity end, add new Activity, 
- [x] If last Activity has CutOff and long enough => last Activity end, add new Activity
- [x] If last Activity has CutOff and long not enough => last Activity End as last Activity Start plus CutOff, add new Activity with End as null, insert between new Activity and last Activity, Activity with UNKNOWN type