using FluentAssertions;
using Nexticz.Module.Vh.Domain.WorkerShifts;
using Xunit;

namespace Nexticz.Module.Vh.Domain.Tests.Unit.WorkerShifts;

public class WorkerShiftSpecifications
{
    [Fact]
    public void CreateWorkerShift_ShouldThrowArgumentNullExceptionIfActivityIsNull_AfterWorkerShiftCreated()
    {
        // Act
        var create = () => new WorkerShift("workerCenterCode", null!){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};

        //Assert
        create.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void CreateWorkerShift_ShouldHaveExactlyOneActivity_AfterWorkerShiftCreated()
    {
        //Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 8, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};

        // Act
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};

        //Assert
        workerShift.Activities.Count.Should().Be(1);
    }

    [Fact]
    public void CreateWorkerShift_ShouldStartsAtSameTimeAsFirstActivity_AfterWorkerShiftCreated()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 8, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};

        // Act
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};

        //Assert
        workerShift.Start.Should().Be(activity.Start);
    }

    [Fact]
    public void CreateWorkerShift_ShouldHaveSameWorkerCodeAsFirstActivity_AfterWorkerShiftCreated()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 8, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};

        // Act
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};

        //Assert
        workerShift.WorkerCode.Should().Be(activity.WorkerCode);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CloseWorkerShift_ShouldApprovedIsSetToFalse_WhenWorkerShiftShouldBeClosed(bool shouldBeClosed)
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 8, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};
        var workerShiftEndDate = new DateTime(2024, 5, 13, 10, 0, 0);
        workerShift.SetApproval(shouldBeClosed);

        // Act
        var close = () => workerShift.CloseWorkerShift(workerShiftEndDate);

        // Assert
        if (shouldBeClosed)
        {
            close.Should().ThrowExactly<InvalidOperationException>();
        }
        else
        {
            close.Should().NotThrow();
            workerShift.End.Should().Be(workerShiftEndDate);
        }
    }

    [Fact]
    public void CloseWorkerShift_ShouldWorkerShiftEndOnEndTime_WhenItShouldBeClosed()
    {
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 8, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};
        var workerShiftEndDate = new DateTime(2024, 5, 13, 10, 0, 0);

        // Act
        workerShift.CloseWorkerShift(workerShiftEndDate);

        // Assert
        workerShift.End.Should().Be(workerShiftEndDate);
    }

    [Fact]
    public void CloseWorkerShift_ShouldLastActivityEndTimeIsSetToWorkerShiftEndTime_IfLastActivityHasNotCutOffTime()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 8, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};
        var workerShiftEndDate = new DateTime(2024, 5, 13, 10, 0, 0);

        // Act
        workerShift.CloseWorkerShift(workerShiftEndDate);

        // Assert
        workerShift.Activities.Last().End.Should().Be(workerShiftEndDate);
    }

    [Fact]
    public void
        CloseWorkerShift_ShouldLastActivityEndTimeIsSetToWorkerShiftEndTime_IfLastActivityHasCutOffTimeUnExceeded()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity.ActivityCutOff = new TimeOnly(0, 30, 0);
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};
        var workerShiftEndDate = new DateTime(2024, 5, 13, 10, 20, 0);

        // Act
        workerShift.CloseWorkerShift(workerShiftEndDate);

        // Assert
        workerShift.Activities.Last().End.Should().Be(workerShiftEndDate);
    }

    [Fact]
    public void
        CloseWorkerShift_ShouldLastActivityEndTimeIsSetToWorkerShiftEndTime_IfLastActivityHasCutOffTimeExceeded()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity.ActivityCutOff = new TimeOnly(0, 10, 0);
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};
        var workerShiftEndDate = new DateTime(2024, 5, 13, 10, 20, 0);

        // Act
        workerShift.CloseWorkerShift(workerShiftEndDate);

        // Assert
        workerShift.Activities.Last().End.Should().Be(workerShiftEndDate);
        workerShift.Activities.Count.Should().Be(2);
        workerShift.Activities.Last().ActivityType.Should().Be(ActivityType.Unknown);
    }

    [Fact]
    public void
        CloseWorkerShift_ShouldLastActivityEndTimeIsSetToWorkerShiftEndTime_IfEndOverOverlappingActivity()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 11, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity2 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 12, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity.ActivityCutOff = new TimeOnly(0, 10, 0);
        activity1.ActivityCutOff = new TimeOnly(0, 10, 0);
        activity2.ActivityCutOff = new TimeOnly(0, 10, 0);
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};
        workerShift.AddActivity(activity1);
        workerShift.AddActivity(activity2);
        var workerShiftEndDate = new DateTime(2024, 5, 13, 11, 20, 0);

        // Act
        workerShift.CloseWorkerShift(workerShiftEndDate);

        // Assert
        workerShift.Activities.Last().End.Should().Be(workerShiftEndDate);
        workerShift.Activities.Count.Should().Be(4);
        workerShift.Activities.Last().ActivityType.Should().Be(ActivityType.Unknown);
    }

    [Fact]
    public void CloseWorkerShift_ShouldWorkerShiftHaveConsistencyDataWithoutSpaces_WhenItShouldBeClosed()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity.ActivityCutOff = new TimeOnly(0, 10, 0);
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};
        var workerShiftEndDate = new DateTime(2024, 5, 13, 10, 20, 0);

        // Act
        workerShift.CloseWorkerShift(workerShiftEndDate);

        // Assert
        workerShift.Activities.Last().ActivityType.Should().Be(ActivityType.Unknown);
        workerShift.Activities.Last().End.Should().Be(workerShiftEndDate);
        workerShift.Activities.Count.Should().Be(2);
    }

    [Fact]
    public void CloseWorkerShift_ShouldNotContainTrimmedOffActivities_WhenItShouldBeClosed()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 11, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity2 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 12, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode2",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity.ActivityCutOff = new TimeOnly(0, 10, 0);
        activity2.ActivityCutOff = new TimeOnly(0, 10, 0);
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};
        workerShift.AddActivity(activity1);
        workerShift.AddActivity(activity2);

        var workerShiftEndDate = new DateTime(2024, 5, 13, 10, 30, 0);

        // Act
        _ = workerShift.CloseWorkerShift(workerShiftEndDate);

        // Assert
        workerShift.Activities.Count.Should().Be(2);
        workerShift.Activities.Should().Contain(activity);
        workerShift.Activities.Last().ActivityType.Should().Be(ActivityType.Unknown);
    }

    [Fact]
    public void CloseWorkerShift_ShouldReturnListOfTrimmedOffActivities_WhenItShouldBeClosed()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 11, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity2 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 12, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode2",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity.ActivityCutOff = new TimeOnly(0, 10, 0);
        activity2.ActivityCutOff = new TimeOnly(0, 10, 0);
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};
        workerShift.AddActivity(activity1);
        workerShift.AddActivity(activity2);

        var workerShiftEndDate = new DateTime(2024, 5, 13, 10, 30, 0);

        // Act
        var trimmedActivities = workerShift.CloseWorkerShift(workerShiftEndDate);

        // Assert
        trimmedActivities.Count.Should().Be(2);
        trimmedActivities.Should().Contain(activity1);
        trimmedActivities.Should().Contain(activity2);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ApproveWorkerShift_ShouldMustBeSetToFalse_BeforeApproval(bool shouldBeApproved)
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity.ActivityCutOff = new TimeOnly(0, 10, 0);
        var workerShift = new WorkerShift("workerCenterCode", activity)
        {
            End = new DateTime(2024, 5, 13, 10, 10, 0),
            ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""
        };

        // Act
        var approve = () => workerShift.SetApproval(shouldBeApproved);

        // Assert
        if (shouldBeApproved)
            approve.Should().ThrowExactly<InvalidOperationException>();
        else
            approve.Should().NotThrow();
    }

    [Fact]
    public void ApproveWorkerShift_ShouldMustNotHaveAnyUnknownActivities_BeforeApproval()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.Unknown,
            ActivitySource.Iwms){ActivityName = ""};
        activity.ActivityCutOff = new TimeOnly(0, 10, 0);
        var workerShift = new WorkerShift("workerCenterCode", activity)
        {
            End = new DateTime(2024, 5, 13, 10, 5, 0),
            ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""
        };

        // Act
        var close = () => workerShift.SetApproval(false);

        // Assert
        close.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void ApproveWorkerShift_MustHaveEndTime_BeforeApproval()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity.ActivityCutOff = new TimeOnly(0, 10, 0);
        var workerShift = new WorkerShift("workerCenterCode", activity)
        {
            End = null,
            ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""
        };

        // Act
        var close = () => workerShift.SetApproval(false);

        // Assert
        close.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void ApproveWorkerShift_ShouldSetApproveToTrueAfterApproval_AfterApproval()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity.ActivityCutOff = new TimeOnly(0, 10, 0);
        var workerShift = new WorkerShift("workerCenterCode", activity)
        {
            End = new DateTime(2024, 5, 13, 10, 5, 0),
            ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""
        };

        // Act
        var approve = () => workerShift.SetApproval(false);

        // Assert
        approve.Should().NotThrow();
        workerShift.Approved.Should().BeTrue();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void DisApproveWorkerShift_ShouldApprovedBeTrue_BeforeUnApproval(bool shouldBeDisApproved)
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity.ActivityCutOff = new TimeOnly(0, 10, 0);
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};

        // Act
        var disApprove = () => workerShift.SetApproval(shouldBeDisApproved);

        // Assert
        if (shouldBeDisApproved)
        {
            disApprove.Should().NotThrow();
            workerShift.Approved.Should().BeFalse();
        }
        else
        {
            disApprove.Should().ThrowExactly<InvalidOperationException>();
        }
    }

    [Fact]
    public void DisApproveWorkerShift_Should_AfterUnApproval()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity.ActivityCutOff = new TimeOnly(0, 10, 0);
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};

        // Act
        var disApprove = () => workerShift.SetApproval(false);

        // Assert
    }

    [Fact]
    public void AddActivity_ShouldApprovedSetToFalse_BeforeAdding()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 11, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};

        workerShift.SetApproval(true);

        // Act
        var added = () => workerShift.AddActivity(activity1);

        // Assert
        added.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void AddActivity_ShouldAddActivity_ToWorkerShift()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 11, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};

        // Act
        workerShift.AddActivity(activity1);

        // Assert
        workerShift.Activities.Count.Should().Be(2);
    }

    [Fact]
    public void AddActivity_ShouldNotAddActivity_IfActivityStartsAfterWorkerShiftEnd()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 11, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};
        var workerShiftEndTime = new DateTime(2024, 5, 13, 10, 30, 0);
        workerShift.CloseWorkerShift(workerShiftEndTime);

        // Act
        var addActivity = () => workerShift.AddActivity(activity1);

        // Assert
        addActivity.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void AddActivity_ShouldNotAddActivity_IfWorkerShiftHasDifferentWorkerCodeThanActivity()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 11, 0, 0),
            "testWorkerCode1",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};

        // Act
        var addActivity = () => workerShift.AddActivity(activity1);

        // Assert
        addActivity.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void AddActivity_ShouldHasConsistentDataWithoutSpaces_AfterActivityAdded()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 11, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity.ActivityCutOff = new TimeOnly(0, 10);
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};

        // Act
        workerShift.AddActivity(activity1);

        // Assert
        workerShift.Activities.Count.Should().Be(3);
    }

    [Fact]
    public void
        AddActivity_ShouldUpdateStartWorkerShiftStartToActivityStart_IfIsAddedActivityBeforeWorkerShiftStart()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 9, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity.ActivityCutOff = new TimeOnly(0, 10);
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};

        // Act
        workerShift.AddActivity(activity1);

        // Assert
        workerShift.Start.Should().Be(activity1.Start);
    }

    [Fact]
    public void
        AddActivityBeforeWorkerShiftStart_ShouldNotAddNewActivitySetFirstActivityStartToNewActivityStartAndIncreaseActivitiesCountPlus1_IfAddedWithSameCodeAndWithoutCutOff()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 9, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};

        // Act
        workerShift.AddActivity(activity1);

        // Assert
        workerShift.Activities.Count.Should().Be(1);
        activity.Start.Should().Be(activity1.Start);
        activity.ActivitiesCount.Should().Be(2);
    }

    [Fact]
    public void
        AddActivityBeforeWorkerShiftStart_ShouldNotAddNewActivitySetFirstActivityStartToNewActivityStartAndIncreaseActivitiesCountPlus1_IfAddedWithSameCodeAndWithCutOffLonger()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 9, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity1.ActivityCutOff = new TimeOnly(1, 10);
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};

        // Act
        workerShift.AddActivity(activity1);

        // Assert
        workerShift.Activities.Count.Should().Be(1);
        activity.Start.Should().Be(activity1.Start);
        activity.ActivitiesCount.Should().Be(2);
    }

    [Fact]
    public void
        AddActivityBeforeWorkerShiftStart_ShouldAddNewActivity_IfAddedWithSameCodeAndWithCutOffShorterButWithoutFollowingActivityWithDifferentType()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 9, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity.ActivityCutOff = new TimeOnly(0, 10);
        activity1.ActivityCutOff = new TimeOnly(0, 10);
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};

        // Act
        workerShift.AddActivity(activity1);

        // Assert
        workerShift.Activities.Count.Should().Be(3);
    }

    [Fact]
    public void
        AddActivityBeforeWorkerShiftStart_ShouldAddNewActivityAndUnknownActivity_IfAddedWithSameCodeAndWithCutOffShorterAndWithFollowingActivityWithDifferentType()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 9, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity2 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 11, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity.ActivityCutOff = new TimeOnly(0, 10);
        activity1.ActivityCutOff = new TimeOnly(0, 10);
        activity2.ActivityCutOff = new TimeOnly(0, 10);
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};
        workerShift.AddActivity(activity1);

        // Act
        workerShift.AddActivity(activity2);

        // Assert
        workerShift.Activities.Count.Should().Be(5);
        workerShift.Activities[1].ActivityType.Should().Be(ActivityType.Unknown);
        workerShift.Activities.First().End.Should().Be(new DateTime(2024, 5, 13, 9, 10, 0));
    }

    [Fact]
    public void
        AddActivityBeforeWorkerShiftStart_ShouldAddNewActivityEndTimeNewActivityIsStartTimeFirstActivity_IfAddedWithDifferentCodeAndWithoutCutOff()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 9, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};

        // Act
        workerShift.AddActivity(activity1);

        // Assert
        workerShift.Activities.Count.Should().Be(2);
        activity1.End.Should().Be(activity.Start);
    }

    [Fact]
    public void
        AddActivityBeforeWorkerShiftStart_ShouldAddNewActivityEndTimeNewActivityIsStartTimeFirstActivity_IfAddedWithDifferentCodeAndWithCutOffLonger()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 9, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity1.ActivityCutOff = new TimeOnly(1, 20);
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};

        // Act
        workerShift.AddActivity(activity1);

        // Assert
        workerShift.Activities.Count.Should().Be(2);
        activity1.End.Should().Be(activity.Start);
    }

    [Fact]
    public void
        AddActivityBeforeWorkerShiftStart_ShouldAddNewActivityAndAddUnknownActivity_IfAddedWithDifferentCodeAndWithCutOffShorter()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 9, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity1.ActivityCutOff = new TimeOnly(0, 20);
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};

        // Act
        workerShift.AddActivity(activity1);

        // Assert
        workerShift.Activities.Count.Should().Be(3);
        activity1.End.Should().Be(new DateTime(2024, 5, 13, 9, 20, 0));
        workerShift.Activities[1].Start.Should().Be(new DateTime(2024, 5, 13, 9, 20, 0));
        workerShift.Activities[1].End.Should().Be(activity.Start);
    }

    [Fact]
    public void
        AddOverlappingActivity_ShouldDoNothing_IfAddedWithSameCode()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 9, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity2 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 9, 30, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};
        workerShift.AddActivity(activity1);

        // Act
        workerShift.AddActivity(activity2);

        // Assert
        workerShift.Activities.Count.Should().Be(2);
        activity.ActivitiesCount.Should().Be(1);
    }

    [Fact]
    public void
        AddOverlappingActivity_ShouldEndOverlappedActivityWithNewActivityStartAddNewActivityWithEndOfNextActivity_IfAddedWithDifferentCodeNextActivityExistAndIsDifferentWithoutCutOff()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 9, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity2 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 11, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode2",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity3 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 30, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode3",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};
        workerShift.AddActivity(activity1);
        workerShift.AddActivity(activity2);

        // Act
        workerShift.AddActivity(activity3);

        // Assert
        activity1.End.Should().Be(activity3.Start);
        activity3.End.Should().Be(activity2.Start);
        workerShift.Activities.Count.Should().Be(4);
    }

    [Fact]
    public void
        AddOverlappingActivity_ShouldEndOverlappedActivityWithNewActivityStartAddNewActivityWithEndOfNextActivity_IfAddedWithDifferentCodeNextActivityExistAndIsDifferentWithNonExcitingCutOff()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 9, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity2 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 11, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode2",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity3 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 30, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode3",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity1.ActivityCutOff = new TimeOnly(1, 30);
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};
        workerShift.AddActivity(activity1);
        workerShift.AddActivity(activity2);

        // Act
        workerShift.AddActivity(activity3);

        // Assert
        activity1.End.Should().Be(activity3.Start);
        activity3.End.Should().Be(activity2.Start);
        workerShift.Activities.Count.Should().Be(4);
    }

    [Fact]
    public void
        AddOverlappingActivity_ShouldAddNewActivityAndUnknownActivityBetween_IfAddedWithDifferentCodeNextActivityExistAndIsDifferentWithExcitingCutOff()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 9, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity2 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 11, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode2",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity3 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 40, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode3",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity1.ActivityCutOff = new TimeOnly(0, 30);
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};
        workerShift.AddActivity(activity1);
        workerShift.AddActivity(activity2);

        // Act
        workerShift.AddActivity(activity3);

        // Assert
        activity1.End.Should().Be(new DateTime(2024, 5, 13, 10, 30, 0));
        workerShift.Activities[2].ActivityType.Should().Be(ActivityType.Unknown);
        workerShift.Activities[2].Start.Should().Be(new DateTime(2024, 5, 13, 10, 30, 0));
        workerShift.Activities[2].End.Should().Be(new DateTime(2024, 5, 13, 10, 40, 0));
        activity3.End.Should().Be(activity2.Start);
        workerShift.Activities.Count.Should().Be(5);
    }

    [Fact]
    public void
        AddOverlappingActivity_ShouldEndOverlappedWithNewStartStartNextActivityStartAsNewActivityNextActivityCountPlus1_IfAddedWithDifferentCodeNextActivityExistAndIsSameWithoutCutOff()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 9, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity2 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 11, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode2",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity3 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 30, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode2",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};
        workerShift.AddActivity(activity1);
        workerShift.AddActivity(activity2);

        // Act
        workerShift.AddActivity(activity3);

        // Assert
        activity1.End.Should().Be(activity3.Start);
        activity2.Start.Should().Be(activity3.Start);
        activity2.ActivitiesCount.Should().Be(2);
        workerShift.Activities.Count.Should().Be(3);
    }

    [Fact]
    public void
        AddOverlappingActivity_ShouldEndOverlappedActivityWithNewActivityStartNextActivityStartAsNewActivityNextActivityCountPlus1_IfAddedWithDifferentCodeNextActivityExistAndIsSameWithNonExcitingCutOff()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 9, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity2 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 11, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode2",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity3 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 30, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode2",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity1.ActivityCutOff = new TimeOnly(1, 30);
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};
        workerShift.AddActivity(activity1);
        workerShift.AddActivity(activity2);

        // Act
        workerShift.AddActivity(activity3);

        // Assert
        activity1.End.Should().Be(activity3.Start);
        activity2.Start.Should().Be(activity3.Start);
        activity2.ActivitiesCount.Should().Be(2);
        workerShift.Activities.Count.Should().Be(3);
    }

    [Fact]
    public void
        AddOverlappingActivity_ShouldEndOverlappedActivityWithNewActivityEndWithStartPlusCutOffINsertBetweenUndefinedActivity_IfAddedWithDifferentCodeNextActivityExistAndIsSameWithExcitingCutOff()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 9, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity2 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 11, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode2",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity4 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 12, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode4",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity3 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 40, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode2",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity1.ActivityCutOff = new TimeOnly(0, 30);
        activity2.ActivityCutOff = new TimeOnly(0, 5);
        activity3.ActivityCutOff = new TimeOnly(0, 5);
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};
        workerShift.AddActivity(activity1);
        workerShift.AddActivity(activity2);
        workerShift.AddActivity(activity4);

        // Act
        workerShift.AddActivity(activity3);

        // Assert
        activity1.End.Should().Be(new DateTime(2024, 5, 13, 10, 30, 0));
        workerShift.Activities[2].ActivityType.Should().Be(ActivityType.Unknown);
        workerShift.Activities[2].Start.Should().Be(new DateTime(2024, 5, 13, 10, 30, 0));
        workerShift.Activities[2].End.Should().Be(new DateTime(2024, 5, 13, 10, 40, 0));
        activity2.End.Should().Be(new DateTime(2024, 5, 13, 11, 05, 0));
        activity2.ActivitiesCount.Should().Be(1);
        workerShift.Activities[4].ActivityType.Should().Be(ActivityType.Unknown);
        workerShift.Activities[4].Start.Should().Be(new DateTime(2024, 5, 13, 10, 45, 0));
        workerShift.Activities[4].End.Should().Be(new DateTime(2024, 5, 13, 11, 0, 0));
        workerShift.Activities.Last().Start.Should().Be(new DateTime(2024, 5, 13, 12, 0, 0));
        workerShift.Activities.Last().End.Should().Be(null);
        workerShift.Activities.Count.Should().Be(8);
    }

    [Fact]
    public void
        AddOverlappingActivity_ShouldEndOverlappedActivityWhenNewStartAndNewEndWithWorkerShiftEnd_IfAddedWithDifferentCodeNextActivityNotExistAndWorkerShiftEndIsSetWithoutCutOff()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 9, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity2 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 30, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode2",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};
        workerShift.AddActivity(activity1);

        // Act
        workerShift.CloseWorkerShift(new DateTime(2024, 5, 13, 11, 0, 0));
        workerShift.AddActivity(activity2);

        // Assert
        activity1.End.Should().Be(activity2.Start);
        activity2.End.Should().Be(workerShift.End);
        workerShift.Activities.Count.Should().Be(3);
    }

    [Fact]
    public void
        AddOverlappingActivity_ShouldEndOverlappedActivityWhenNewStartAndNewEndWithWorkerShiftEnd_IfAddedWithDifferentCodeNextActivityNotExistAndWorkerShiftEndIsSetWithNonExcitingCutOff()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 9, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity2 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 30, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode2",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity1.ActivityCutOff = new TimeOnly(1, 10);
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};
        workerShift.AddActivity(activity1);

        // Act
        workerShift.CloseWorkerShift(new DateTime(2024, 5, 13, 11, 0, 0));
        workerShift.AddActivity(activity2);

        // Assert
        activity1.End.Should().Be(activity2.Start);
        activity2.End.Should().Be(workerShift.End);
        workerShift.Activities.Count.Should().Be(3);
    }

    [Fact]
    public void
        AddOverlappingActivity_ShouldEndOverlappedActivityWhenNewStartAndNewEndWithStartEndCutOffAndInsertUnknownActivityBetweenWorkerShiftEnd_IfAddedWithDifferentCodeNextActivityNotExistAndWorkerShiftEndIsSetWithExcitingCutOff()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 9, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity2 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 30, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode2",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity1.ActivityCutOff = new TimeOnly(0, 10);
        activity2.ActivityCutOff = new TimeOnly(0, 10);
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};
        workerShift.AddActivity(activity1);

        // Act
        workerShift.CloseWorkerShift(new DateTime(2024, 5, 13, 11, 0, 0));
        workerShift.AddActivity(activity2);

        // Assert
        activity1.End.Should().Be(new DateTime(2024, 5, 13, 10, 10, 0));
        activity2.End.Should().Be(new DateTime(2024, 5, 13, 10, 40, 0));
        workerShift.Activities.Count(x => x.ActivityType == ActivityType.Unknown).Should().Be(2);
        workerShift.Activities.Count.Should().Be(5);
    }

    [Fact]
    public void
        AddActivityToEnd_ShouldLastActivityCountIncreasePlus1_IfAddedWithSameCodeWithoutCutOff()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 9, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity2 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 30, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};
        workerShift.AddActivity(activity1);

        // Act
        workerShift.AddActivity(activity2);

        // Assert
        activity1.ActivitiesCount.Should().Be(2);
        activity1.End.Should().Be(null);
        workerShift.Activities.Count.Should().Be(2);
    }

    [Fact]
    public void
        AddActivityToEnd_ShouldLastActivityCountIncreasePlus1_IfAddedWithSameCodeWithNonExcitingCutOff()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 9, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity2 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 30, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity1.ActivityCutOff = new TimeOnly(1, 0);
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};
        workerShift.AddActivity(activity1);

        // Act
        workerShift.AddActivity(activity2);

        // Assert
        activity1.ActivitiesCount.Should().Be(2);
        activity1.End.Should().Be(null);
        workerShift.Activities.Count.Should().Be(2);
    }

    [Fact]
    public void
        AddActivityToEnd_ShouldLastActivityEndWithStartPlusCutOffAddNewActivityWithNullEndAndInserBetweanUnknownActivity_IfAddedWithSameCodeWithExcitingCutOff()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 9, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity2 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 30, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity1.ActivityCutOff = new TimeOnly(0, 10);
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};
        workerShift.AddActivity(activity1);

        // Act
        workerShift.AddActivity(activity2);

        // Assert
        activity1.ActivitiesCount.Should().Be(1);
        activity2.End.Should().Be(null);
        workerShift.Activities.Count.Should().Be(4);
    }

    [Fact]
    public void
        AddActivityToEnd_ShouldAddNewActivity_IfAddedWithDifferentCodeWithoutCutOff()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 9, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity2 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 30, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode2",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};
        workerShift.AddActivity(activity1);

        // Act
        workerShift.AddActivity(activity2);

        // Assert
        activity1.ActivitiesCount.Should().Be(1);
        activity1.End.Should().Be(activity2.Start);
        workerShift.Activities.Count.Should().Be(3);
    }

    [Fact]
    public void
        AddActivityToEnd_ShouldAddNewActivity_IfAddedWithDifferentCodeWithNonExcitingCutOff()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 9, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity2 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 30, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode2",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity1.ActivityCutOff = new TimeOnly(1, 0);
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};
        workerShift.AddActivity(activity1);

        // Act
        workerShift.AddActivity(activity2);

        // Assert
        activity1.ActivitiesCount.Should().Be(1);
        activity1.End.Should().Be(activity2.Start);
        workerShift.Activities.Count.Should().Be(3);
    }

    [Fact]
    public void
        AddActivityToEnd_ShouldLastActivityEndWithStartAndCutOffAddNewActivityWithNullEndAndInsertBetweenUnknownActivity_IfAddedWithDifferentCodeWithExcitingCutOff()
    {
        // Arrange
        var activity = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 9, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity1 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 0, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode1",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        var activity2 = new WorkerShiftActivity(
            new DateTime(2024, 5, 13, 10, 30, 0),
            "testWorkerCode",
            "testCenterCode",
            "testActivityCode2",
            null,
            null,
            "",
            ActivityType.PaidIwms,
            ActivitySource.Iwms){ActivityName = ""};
        activity1.ActivityCutOff = new TimeOnly(0, 10);
        var workerShift = new WorkerShift("workerCenterCode", activity){ActivityAfterCutOffCode = "", ActivityAfterCutOffName = ""};
        workerShift.AddActivity(activity1);

        // Act
        workerShift.AddActivity(activity2);

        // Assert
        activity1.ActivitiesCount.Should().Be(1);
        activity1.End.Should().Be(new DateTime(2024, 5, 13, 10, 10, 0));
        workerShift.Activities[2].ActivityType.Should().Be(ActivityType.Unknown);
        workerShift.Activities[2].Start.Should().Be(new DateTime(2024, 5, 13, 10, 10, 0));
        workerShift.Activities[2].End.Should().Be(new DateTime(2024, 5, 13, 10, 30, 0));
        workerShift.Activities.Count.Should().Be(4);
    }
}