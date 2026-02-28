
using ErrorOr;
using Nexticz.Module.Mmo.Reporting.Application.LineItems.LineItemQueueBuilder;
using Nexticz.Module.Mmo.Reporting.Domain.ShiftAggregate;
using Nexticz.Module.Mmo.Reporting.Domain.Views;
using Shouldly;

namespace Nexticz.Module.Mmo.Reporting.Tests.Unit.Application;

public class LineItemQueueBuilderTests
{
    [Fact]
    public async Task BuildAsync_WhenCalledWithPlannedLineItems_ReturnsQueueInCorrectOrder()
    {
        var lastItemEndDateOnLine = new DateTime(2025, 6, 20, 4, 0, 0);
        var kitStartDate = new DateTime(2025, 6, 20, 14, 0, 0);
        var kitEndDate = new DateTime(2025, 6, 20, 18, 0, 0);
        var optimalKitDuration = new TimeSpan(4, 0, 0);
        const int adjustmentTimInMinutes = 60;

        var plannedLineItems = new List<LineItemView>
        {
            GenerateLineItemView(
                new DateTime(2025, 6, 20, 15, 0, 0),
                new DateTime(2025, 6, 20, 16, 0, 0))
        };
        
        var shiftId = Guid.NewGuid();
        
        // Act
        var result = 
            await LineItemQueueBuilder.BuildAsync(CreateShift(shiftId), kitStartDate, kitEndDate, optimalKitDuration, GenerateLastItemPerView(lastItemEndDateOnLine, shiftId), adjustmentTimInMinutes, plannedLineItems.ToArray(), CreateNextShiftFunc());

        result.LineItemsToCreateOrUpdate.Count.ShouldBe(5);
        result.LineItemsToCreateOrUpdate[0].StartDate.ShouldBe(new DateTime(2025, 6, 20, 4, 0, 0));
        result.LineItemsToCreateOrUpdate[0].EndDate.ShouldBe(new DateTime(2025, 6, 20, 13, 0, 0));
        result.LineItemsToCreateOrUpdate[0].Type.ShouldBe(LineItemType.Downtime);
        
        result.LineItemsToCreateOrUpdate[1].StartDate.ShouldBe(new DateTime(2025, 6, 20, 13, 0, 0));
        result.LineItemsToCreateOrUpdate[1].EndDate.ShouldBe(new DateTime(2025, 6, 20, 14, 0, 0));
        result.LineItemsToCreateOrUpdate[1].Type.ShouldBe(LineItemType.Adjustment);
        
        result.LineItemsToCreateOrUpdate[2].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[2].StartDate.ShouldBe(new DateTime(2025, 6, 20, 14, 0, 0));
        
        result.LineItemsToCreateOrUpdate[3].Type.ShouldBe(LineItemType.Break);
        result.LineItemsToCreateOrUpdate[3].StartDate.ShouldBe(new DateTime(2025, 6, 20, 15, 0, 0));
        
        result.LineItemsToCreateOrUpdate[4].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[4].StartDate.ShouldBe(new DateTime(2025, 6, 20, 16, 0, 0));
        result.LineItemsToCreateOrUpdate[4].EndDate.ShouldBe(new DateTime(2025, 6, 20, 18, 0, 0));
        
        result.LineItemToShorten.ShouldBeNull();
    }
    
    [Fact]
    public async Task BuildAsync_WhenCalledWithPlannedLineItems_ReturnsQueueInCorrectOrderWithCorrectTotalKitTime()
    {
        var lastItemEndDateOnLine = new DateTime(2025, 6, 20, 4, 0, 0);
        var kitStartDate = new DateTime(2025, 6, 20, 14, 0, 0);
        var kitEndDate = new DateTime(2025, 6, 20, 18, 0, 0);
        var optimalKitDuration = new TimeSpan(4, 0, 0);
        const int adjustmentTimInMinutes = 60;

        var plannedLineItems = new List<LineItemView>
        {
            GenerateLineItemView(
                new DateTime(2025, 6, 20, 15, 0, 0),
                new DateTime(2025, 6, 20, 16, 0, 0)),
            GenerateLineItemView(
                new DateTime(2025, 6, 20, 13, 10, 0),
                new DateTime(2025, 6, 20, 13, 30, 0)),
            GenerateLineItemView(
                new DateTime(2025, 6, 20, 5, 0, 0),
                new DateTime(2025, 6, 20, 6, 0, 0))
        };
        
        var shiftId = Guid.NewGuid();
        
        // Act
        var result = 
            await LineItemQueueBuilder.BuildAsync(CreateShift(shiftId), kitStartDate, kitEndDate, optimalKitDuration, GenerateLastItemPerView(lastItemEndDateOnLine, shiftId), adjustmentTimInMinutes, plannedLineItems.ToArray(), CreateNextShiftFunc());

        result.LineItemsToCreateOrUpdate.Count.ShouldBe(9);
        
        result.LineItemsToCreateOrUpdate[0].Type.ShouldBe(LineItemType.Downtime);
        result.LineItemsToCreateOrUpdate[0].StartDate.ShouldBe(new DateTime(2025, 6, 20, 4, 0, 0));
        result.LineItemsToCreateOrUpdate[0].ExistingId.ShouldBeNull();
        
        result.LineItemsToCreateOrUpdate[1].Type.ShouldBe(LineItemType.Break);
        result.LineItemsToCreateOrUpdate[1].StartDate.ShouldBe(new DateTime(2025, 6, 20, 5, 0, 0));
        result.LineItemsToCreateOrUpdate[1].ExistingId.ShouldNotBeNull();
        
        result.LineItemsToCreateOrUpdate[2].Type.ShouldBe(LineItemType.Downtime);
        result.LineItemsToCreateOrUpdate[2].StartDate.ShouldBe(new DateTime(2025, 6, 20, 6, 0, 0));
        result.LineItemsToCreateOrUpdate[2].ExistingId.ShouldBeNull();
        
        result.LineItemsToCreateOrUpdate[3].Type.ShouldBe(LineItemType.Adjustment);
        result.LineItemsToCreateOrUpdate[3].StartDate.ShouldBe(new DateTime(2025, 6, 20, 12, 40, 0));
        result.LineItemsToCreateOrUpdate[3].ExistingId.ShouldBeNull();
        
        result.LineItemsToCreateOrUpdate[4].Type.ShouldBe(LineItemType.Break);
        result.LineItemsToCreateOrUpdate[4].StartDate.ShouldBe(new DateTime(2025, 6, 20, 13, 10, 0));
        result.LineItemsToCreateOrUpdate[4].ExistingId.ShouldNotBeNull();
        
        result.LineItemsToCreateOrUpdate[5].Type.ShouldBe(LineItemType.Adjustment);
        result.LineItemsToCreateOrUpdate[5].StartDate.ShouldBe(new DateTime(2025, 6, 20, 13, 30, 0));
        
        result.LineItemsToCreateOrUpdate[6].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[6].StartDate.ShouldBe(new DateTime(2025, 6, 20, 14, 0, 0));
        result.LineItemsToCreateOrUpdate[6].ExistingId.ShouldBeNull();
        result.LineItemsToCreateOrUpdate[6].TotalDuration.Hours.ShouldBe(3);
        
        result.LineItemsToCreateOrUpdate[7].Type.ShouldBe(LineItemType.Break);
        result.LineItemsToCreateOrUpdate[7].StartDate.ShouldBe(new DateTime(2025, 6, 20, 15, 0, 0));
        result.LineItemsToCreateOrUpdate[7].ExistingId.ShouldNotBeNull();
        
        result.LineItemsToCreateOrUpdate[8].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[8].StartDate.ShouldBe(new DateTime(2025, 6, 20, 16, 0, 0));
        result.LineItemsToCreateOrUpdate[8].EndDate.ShouldBe(new DateTime(2025, 6, 20, 18, 0, 0));
        result.LineItemsToCreateOrUpdate[8].ExistingId.ShouldBeNull();
        result.LineItemsToCreateOrUpdate[8].TotalDuration.Hours.ShouldBe(3);
        
        result.LineItemToShorten.ShouldBeNull();
    }
    
    [Fact]
    public async Task BuildAsync_WhenCalledWithPlannedLineItemsWithFirstKitInShiftOnLines_ReturnsQueueInCorrectOrderWithSplitAdjustmentAndSplitDowntime()
    {
        var lastItemEndDateOnLine = new DateTime(2025, 6, 20, 4, 0, 0);
        var kitStartDate = new DateTime(2025, 6, 20, 14, 0, 0);
        var kitEndDate = new DateTime(2025, 6, 20, 18, 0, 0);
        var optimalKitDuration = new TimeSpan(4, 0, 0);
        const int adjustmentTimInMinutes = 60;

        var plannedLineItems = new List<LineItemView>
        {
            GenerateLineItemView(
                new DateTime(2025, 6, 20, 14, 0, 0),
                new DateTime(2025, 6, 20, 16, 0, 0)),
            GenerateLineItemView(
                new DateTime(2025, 6, 20, 13, 10, 0),
                new DateTime(2025, 6, 20, 13, 30, 0)),
            GenerateLineItemView(
                new DateTime(2025, 6, 20, 4, 0, 0),
                new DateTime(2025, 6, 20, 6, 0, 0))
        };
        
        var shiftId = Guid.NewGuid();
        
        // Act
        var result = 
            await LineItemQueueBuilder.BuildAsync(CreateShift(shiftId), kitStartDate, kitEndDate, optimalKitDuration, GenerateLastItemPerView(lastItemEndDateOnLine, shiftId), adjustmentTimInMinutes, plannedLineItems.ToArray(), CreateNextShiftFunc());

        result.LineItemsToCreateOrUpdate.Count.ShouldBe(7);
        
        result.LineItemsToCreateOrUpdate[0].Type.ShouldBe(LineItemType.Break);
        result.LineItemsToCreateOrUpdate[0].StartDate.ShouldBe(new DateTime(2025, 6, 20, 4, 0, 0));
        result.LineItemsToCreateOrUpdate[0].ExistingId.ShouldNotBeNull();
        
        result.LineItemsToCreateOrUpdate[1].Type.ShouldBe(LineItemType.Downtime);
        result.LineItemsToCreateOrUpdate[1].StartDate.ShouldBe(new DateTime(2025, 6, 20, 6, 0, 0));
        result.LineItemsToCreateOrUpdate[1].ExistingId.ShouldBeNull();
        
        result.LineItemsToCreateOrUpdate[2].Type.ShouldBe(LineItemType.Adjustment);
        result.LineItemsToCreateOrUpdate[2].StartDate.ShouldBe(new DateTime(2025, 6, 20, 12, 40, 0));
        result.LineItemsToCreateOrUpdate[2].ExistingId.ShouldBeNull();
        
        result.LineItemsToCreateOrUpdate[3].Type.ShouldBe(LineItemType.Break);
        result.LineItemsToCreateOrUpdate[3].StartDate.ShouldBe(new DateTime(2025, 6, 20, 13, 10, 0));
        result.LineItemsToCreateOrUpdate[3].ExistingId.ShouldNotBeNull();
        
        result.LineItemsToCreateOrUpdate[4].Type.ShouldBe(LineItemType.Adjustment);
        result.LineItemsToCreateOrUpdate[4].StartDate.ShouldBe(new DateTime(2025, 6, 20, 13, 30, 0));
        
        result.LineItemsToCreateOrUpdate[5].Type.ShouldBe(LineItemType.Break);
        result.LineItemsToCreateOrUpdate[5].StartDate.ShouldBe(new DateTime(2025, 6, 20, 14, 0, 0));
        result.LineItemsToCreateOrUpdate[5].ExistingId.ShouldNotBeNull();
        
        result.LineItemsToCreateOrUpdate[6].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[6].StartDate.ShouldBe(new DateTime(2025, 6, 20, 16, 0, 0));
        result.LineItemsToCreateOrUpdate[6].EndDate.ShouldBe(new DateTime(2025, 6, 20, 18, 0, 0));
        result.LineItemsToCreateOrUpdate[6].ExistingId.ShouldBeNull();
        
        result.LineItemToShorten.ShouldBeNull();
    }
    
    [Fact]
    public async Task BuildAsync_WhenCalledWithPlannedLineItems_ReturnsQueueInCorrectOrderAndWithItemToShorten()
    {
        var lastItemEndDateOnLine = new DateTime(2025, 6, 20, 4, 0, 0);
        var kitStartDate = new DateTime(2025, 6, 20, 14, 0, 0);
        var kitEndDate = new DateTime(2025, 6, 20, 18, 0, 0);
        var optimalKitDuration = new TimeSpan(4, 0, 0);
        const int adjustmentTimInMinutes = 60;

        var plannedLineItems = new List<LineItemView>
        {
            GenerateLineItemView(
                new DateTime(2025, 6, 20, 15, 0, 0),
                new DateTime(2025, 6, 20, 16, 0, 0)),
            GenerateLineItemView(
                new DateTime(2025, 6, 20, 17, 30, 0),
                new DateTime(2025, 6, 20, 18, 30, 0)),
            GenerateLineItemView(
                new DateTime(2025, 6, 20, 13, 10, 0),
                new DateTime(2025, 6, 20, 13, 30, 0)),
            GenerateLineItemView(
                new DateTime(2025, 6, 20, 5, 0, 0),
                new DateTime(2025, 6, 20, 6, 0, 0))
        };
        
        var shiftId = Guid.NewGuid();
        
        // Act
        var result = 
            await LineItemQueueBuilder.BuildAsync(CreateShift(shiftId), kitStartDate, kitEndDate, optimalKitDuration, GenerateLastItemPerView(lastItemEndDateOnLine, shiftId), adjustmentTimInMinutes, plannedLineItems.ToArray(), CreateNextShiftFunc());

        result.LineItemsToCreateOrUpdate.Count.ShouldBe(9);
        
        result.LineItemsToCreateOrUpdate[8].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[8].StartDate.ShouldBe(new DateTime(2025, 6, 20, 16, 0, 0));
        result.LineItemsToCreateOrUpdate[8].EndDate.ShouldBe(new DateTime(2025, 6, 20, 18, 0, 0));
        
        result.LineItemToShorten.ShouldNotBeNull();
        result.LineItemToShorten.StartDate.ShouldBe(new DateTime(2025, 6, 20, 18, 0, 0));
    }
    
    [Fact]
    public async Task BuildAsync_WhenCalledWithNoPlannedLineItems_ReturnsQueueInCorrectOrder()
    {
        var lastItemEndDateOnLine = new DateTime(2025, 6, 20, 4, 0, 0);
        var kitStartDate = new DateTime(2025, 6, 20, 14, 0, 0);
        var kitEndDate = new DateTime(2025, 6, 20, 18, 0, 0);
        var optimalKitDuration = new TimeSpan(4, 0, 0);
        const int adjustmentTimInMinutes = 60;
        
        var shiftId = Guid.NewGuid();
        
        // Act
        var result = 
            await LineItemQueueBuilder.BuildAsync(CreateShift(shiftId), kitStartDate, kitEndDate, optimalKitDuration, GenerateLastItemPerView(lastItemEndDateOnLine, shiftId), adjustmentTimInMinutes, [], CreateNextShiftFunc());

        result.LineItemsToCreateOrUpdate.Count.ShouldBe(3);
        
        result.LineItemsToCreateOrUpdate[0].Type.ShouldBe(LineItemType.Downtime);
        result.LineItemsToCreateOrUpdate[0].StartDate.ShouldBe(new DateTime(2025, 6, 20, 4, 0, 0));
        
        result.LineItemsToCreateOrUpdate[1].Type.ShouldBe(LineItemType.Adjustment);
        result.LineItemsToCreateOrUpdate[1].StartDate.ShouldBe(new DateTime(2025, 6, 20, 13, 0, 0));
        
        result.LineItemsToCreateOrUpdate[2].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[2].StartDate.ShouldBe(new DateTime(2025, 6, 20, 14, 0, 0));
        result.LineItemsToCreateOrUpdate[2].EndDate.ShouldBe(new DateTime(2025, 6, 20, 18, 0, 0));
        
        result.LineItemToShorten.ShouldBeNull();
    }
    
    [Fact]
    public async Task BuildAsync_WhenCalledWithOnePlannedLineItemsOverEndDate_ReturnsQueueInCorrectOrderAndWithItemToShorten()
    {
        var lastItemEndDateOnLine = new DateTime(2025, 6, 20, 4, 0, 0);
        var kitStartDate = new DateTime(2025, 6, 20, 14, 0, 0);
        var kitEndDate = new DateTime(2025, 6, 20, 18, 0, 0);
        var optimalKitDuration = new TimeSpan(4, 0, 0);
        const int adjustmentTimInMinutes = 60;
        
        var plannedLineItems = new List<LineItemView>
        {
            GenerateLineItemView(
                new DateTime(2025, 6, 20, 16, 0, 0),
                new DateTime(2025, 6, 20, 19, 0, 0))
        };
        
        var shiftId = Guid.NewGuid();
        
        // Act
        var result = 
            await LineItemQueueBuilder.BuildAsync(CreateShift(shiftId), kitStartDate, kitEndDate, optimalKitDuration, GenerateLastItemPerView(lastItemEndDateOnLine, shiftId), adjustmentTimInMinutes, plannedLineItems.ToArray(), CreateNextShiftFunc());

        result.LineItemsToCreateOrUpdate.Count.ShouldBe(3);
        
        result.LineItemsToCreateOrUpdate[0].Type.ShouldBe(LineItemType.Downtime);
        result.LineItemsToCreateOrUpdate[0].StartDate.ShouldBe(new DateTime(2025, 6, 20, 4, 0, 0));
        
        result.LineItemsToCreateOrUpdate[1].Type.ShouldBe(LineItemType.Adjustment);
        result.LineItemsToCreateOrUpdate[1].StartDate.ShouldBe(new DateTime(2025, 6, 20, 13, 0, 0));
        
        result.LineItemsToCreateOrUpdate[2].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[2].StartDate.ShouldBe(new DateTime(2025, 6, 20, 14, 0, 0));
        result.LineItemsToCreateOrUpdate[2].EndDate.ShouldBe(new DateTime(2025, 6, 20, 18, 0, 0));
        
        result.LineItemToShorten.ShouldNotBeNull();
        result.LineItemToShorten.StartDate.ShouldBe(new DateTime(2025, 6, 20, 18, 0, 0));
    }
    
    [Fact]
    public async Task BuildAsync_WhenCalled_ReturnsQueueInCorrectOrder()
    {
        var lastItemEndDateOnLine = new DateTime(2025, 6, 20, 4, 0, 0);
        var kitStartDate = new DateTime(2025, 6, 20, 14, 0, 0);
        var kitEndDate = new DateTime(2025, 6, 20, 18, 0, 0);
        var optimalKitDuration = new TimeSpan(4, 0, 0);
        const int adjustmentTimInMinutes = 60;
        
        var plannedLineItems = new List<LineItemView>
        {
            GenerateLineItemView(
                new DateTime(2025, 6, 20, 3, 0, 0),
                new DateTime(2025, 6, 20, 5, 0, 0)),
            GenerateLineItemView(
                new DateTime(2025, 6, 20, 16, 0, 0),
                new DateTime(2025, 6, 20, 19, 0, 0))
        };
        
        var shiftId = Guid.NewGuid();
        
        // Act
        var result = 
            await LineItemQueueBuilder.BuildAsync(CreateShift(shiftId), kitStartDate, kitEndDate, optimalKitDuration, GenerateLastItemPerView(lastItemEndDateOnLine, shiftId), adjustmentTimInMinutes, plannedLineItems.ToArray(), CreateNextShiftFunc());

        result.LineItemsToCreateOrUpdate.Count.ShouldBe(4);
        
        result.LineItemsToCreateOrUpdate[0].Type.ShouldBe(LineItemType.Break);
        result.LineItemsToCreateOrUpdate[0].StartDate.ShouldBe(new DateTime(2025, 6, 20, 4, 0, 0));
        
        result.LineItemsToCreateOrUpdate[1].Type.ShouldBe(LineItemType.Downtime);
        result.LineItemsToCreateOrUpdate[1].StartDate.ShouldBe(new DateTime(2025, 6, 20, 5, 0, 0));
        
        result.LineItemsToCreateOrUpdate[2].Type.ShouldBe(LineItemType.Adjustment);
        result.LineItemsToCreateOrUpdate[2].StartDate.ShouldBe(new DateTime(2025, 6, 20, 13, 0, 0));
        
        result.LineItemsToCreateOrUpdate[3].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[3].StartDate.ShouldBe(new DateTime(2025, 6, 20, 14, 0, 0));
        result.LineItemsToCreateOrUpdate[3].EndDate.ShouldBe(new DateTime(2025, 6, 20, 18, 0, 0));
        
        result.LineItemToShorten.ShouldNotBeNull();
        result.LineItemToShorten.StartDate.ShouldBe(new DateTime(2025, 6, 20, 18, 0, 0));
    }
    
    [Fact]
    public async Task BuildAsync_WhenCalledWithLastItemEndDateAfterKitStartDate_ReturnsKitWithLaterEndDate()
    {
        var lastItemEndDateOnLine = new DateTime(2025, 6, 20, 15, 0, 0);
        var kitStartDate = new DateTime(2025, 6, 20, 14, 0, 0);
        var kitEndDate = new DateTime(2025, 6, 20, 18, 0, 0);
        var optimalKitDuration = new TimeSpan(4, 0, 0);
        const int adjustmentTimInMinutes = 60;
        
        var shiftId = Guid.NewGuid();
        
        // Act
        var result = 
            await LineItemQueueBuilder.BuildAsync(CreateShift(shiftId), kitStartDate, kitEndDate, optimalKitDuration, GenerateLastItemPerView(lastItemEndDateOnLine, shiftId), adjustmentTimInMinutes, [], CreateNextShiftFunc());

        result.LineItemsToCreateOrUpdate.Count.ShouldBe(1);
        
        result.LineItemsToCreateOrUpdate[0].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[0].StartDate.ShouldBe(new DateTime(2025, 6, 20, 15, 0, 0));
        result.LineItemsToCreateOrUpdate[0].EndDate.ShouldBe(new DateTime(2025, 6, 20, 18, 0, 0));
        result.LineItemsToCreateOrUpdate[0].ExistingId.ShouldBeNull();
        
        result.LineItemToShorten.ShouldBeNull();
    }
    
    [Fact]
    public async Task BuildAsync_WhenCalledWith2PlannedLineItemsInOneKit_ReturnsQueueInCorrectOrder()
    {
        var lastItemEndDateOnLine = new DateTime(2025, 6, 20, 4, 0, 0);
        var kitStartDate = new DateTime(2025, 6, 20, 14, 0, 0);
        var kitEndDate = new DateTime(2025, 6, 20, 20, 0, 0);
        var optimalKitDuration = new TimeSpan(4, 0, 0);
        const int adjustmentTimInMinutes = 60;

        var plannedLineItems = new List<LineItemView>
        {
            GenerateLineItemView(
                new DateTime(2025, 6, 20, 15, 0, 0),
                new DateTime(2025, 6, 20, 16, 0, 0)),
            GenerateLineItemView(
                new DateTime(2025, 6, 20, 17, 0, 0),
                new DateTime(2025, 6, 20, 18, 0, 0)),
        };
        
        var shiftId = Guid.NewGuid();
        
        // Act
        var result = 
            await LineItemQueueBuilder.BuildAsync(CreateShift(shiftId), kitStartDate, kitEndDate, optimalKitDuration, GenerateLastItemPerView(lastItemEndDateOnLine, shiftId), adjustmentTimInMinutes, plannedLineItems.ToArray(), CreateNextShiftFunc());

        result.LineItemsToCreateOrUpdate.Count.ShouldBe(7);
        
        result.LineItemsToCreateOrUpdate[0].Type.ShouldBe(LineItemType.Downtime);
        result.LineItemsToCreateOrUpdate[0].StartDate.ShouldBe(new DateTime(2025, 6, 20, 4, 0, 0));
        result.LineItemsToCreateOrUpdate[0].ExistingId.ShouldBeNull();
        
        result.LineItemsToCreateOrUpdate[1].Type.ShouldBe(LineItemType.Adjustment);
        result.LineItemsToCreateOrUpdate[1].StartDate.ShouldBe(new DateTime(2025, 6, 20, 13, 0, 0));
        result.LineItemsToCreateOrUpdate[1].ExistingId.ShouldBeNull();
        
        result.LineItemsToCreateOrUpdate[2].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[2].StartDate.ShouldBe(new DateTime(2025, 6, 20, 14, 0, 0));
        result.LineItemsToCreateOrUpdate[2].ExistingId.ShouldBeNull();
        
        result.LineItemsToCreateOrUpdate[3].Type.ShouldBe(LineItemType.Break);
        result.LineItemsToCreateOrUpdate[3].StartDate.ShouldBe(new DateTime(2025, 6, 20, 15, 0, 0));
        result.LineItemsToCreateOrUpdate[3].ExistingId.ShouldNotBeNull();
        
        result.LineItemsToCreateOrUpdate[4].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[4].StartDate.ShouldBe(new DateTime(2025, 6, 20, 16, 0, 0));
        result.LineItemsToCreateOrUpdate[4].ExistingId.ShouldBeNull();
        
        result.LineItemsToCreateOrUpdate[5].Type.ShouldBe(LineItemType.Break);
        result.LineItemsToCreateOrUpdate[5].StartDate.ShouldBe(new DateTime(2025, 6, 20, 17, 0, 0));
        result.LineItemsToCreateOrUpdate[5].ExistingId.ShouldNotBeNull();
        
        result.LineItemsToCreateOrUpdate[6].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[6].StartDate.ShouldBe(new DateTime(2025, 6, 20, 18, 0, 0));
        result.LineItemsToCreateOrUpdate[6].EndDate.ShouldBe(new DateTime(2025, 6, 20, 20, 0, 0));
        result.LineItemsToCreateOrUpdate[6].ExistingId.ShouldBeNull();
        result.LineItemsToCreateOrUpdate[6].TotalDuration.Hours.ShouldBe(4);
        
        result.LineItemToShorten.ShouldBeNull();
    }
    
    [Fact]
    public async Task BuildAsync_FirstKitInShiftOnLine_FollowingShifts_ReturnsQueueInCorrectOrder()
    {
        var startPreviousShift = new DateTimeOffset(2025, 6, 20, 2, 0, 0, TimeSpan.FromHours(2));
        var endPreviousShift = new DateTimeOffset(2025, 6, 20, 6, 0, 0, TimeSpan.FromHours(2));
        var previousShift = CreateShift(Guid.NewGuid(), lastShift: false, new ShiftSchedule(startPreviousShift, endPreviousShift));
        var lastItemEndDateOnLine = new DateTime(2025, 6, 20, 5, 0, 0);
        var funcReturnPreviousShift = CreateNextShiftFunc(previousShift);
        var lastItemOnPreviousShift = GenerateLastItemPerView(lastItemEndDateOnLine, previousShift.Id);
        
        var startCurrentShift = new DateTimeOffset(2025, 6, 20, 6, 0, 0, TimeSpan.FromHours(2));
        var endCurrentShift = new DateTimeOffset(2025, 6, 20, 18, 0, 0, TimeSpan.FromHours(2));
        var currentShift = CreateShift(Guid.NewGuid(), lastShift: true, new ShiftSchedule(startCurrentShift, endCurrentShift));
        
        
        var kitStartDate = new DateTime(2025, 6, 20, 8, 0, 0);
        var kitEndDate = new DateTime(2025, 6, 20, 11, 0, 0);
        var optimalKitDuration = new TimeSpan(3, 0, 0);
        const int adjustmentTimInMinutes = 60;
        
        var plannedLineItems = new List<LineItemView>
        {
            GenerateLineItemView(
                new DateTime(2025, 6, 20, 9, 0, 0),
                new DateTime(2025, 6, 20, 10, 0, 0))
        };
        
        // Act
        var result = 
            await LineItemQueueBuilder.BuildAsync(currentShift, kitStartDate, kitEndDate, optimalKitDuration, lastItemOnPreviousShift, adjustmentTimInMinutes, plannedLineItems.ToArray(), funcReturnPreviousShift);

        result.LineItemsToCreateOrUpdate.Count.ShouldBe(6);
        
        result.LineItemsToCreateOrUpdate[0].Type.ShouldBe(LineItemType.Downtime);
        result.LineItemsToCreateOrUpdate[0].StartDate.ShouldBe(new DateTime(2025, 6, 20, 5, 0, 0));
        result.LineItemsToCreateOrUpdate[0].ShiftId.ShouldBe(previousShift.Id);
        
        result.LineItemsToCreateOrUpdate[1].Type.ShouldBe(LineItemType.Downtime);
        result.LineItemsToCreateOrUpdate[1].StartDate.ShouldBe(new DateTime(2025, 6, 20, 6, 0, 0));
        result.LineItemsToCreateOrUpdate[1].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemsToCreateOrUpdate[2].Type.ShouldBe(LineItemType.Adjustment);
        result.LineItemsToCreateOrUpdate[2].StartDate.ShouldBe(new DateTime(2025, 6, 20, 7, 0, 0));
        result.LineItemsToCreateOrUpdate[2].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemsToCreateOrUpdate[3].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[3].StartDate.ShouldBe(new DateTime(2025, 6, 20, 8, 0, 0));
        result.LineItemsToCreateOrUpdate[3].EndDate.ShouldBe(new DateTime(2025, 6, 20, 9, 0, 0));
        result.LineItemsToCreateOrUpdate[3].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemsToCreateOrUpdate[4].Type.ShouldBe(LineItemType.Break);
        result.LineItemsToCreateOrUpdate[4].StartDate.ShouldBe(new DateTime(2025, 6, 20, 9, 0, 0));
        result.LineItemsToCreateOrUpdate[4].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemsToCreateOrUpdate[5].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[5].StartDate.ShouldBe(new DateTime(2025, 6, 20, 10, 0, 0));
        result.LineItemsToCreateOrUpdate[5].EndDate.ShouldBe(new DateTime(2025, 6, 20, 11, 0, 0));
        result.LineItemsToCreateOrUpdate[5].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemToShorten.ShouldBeNull();
    }
    
    [Fact]
    public async Task BuildAsync_FirstKitInShiftOnLine_FollowingShiftsWithBreakInPreviousShift_ReturnsQueueInCorrectOrder()
    {
        
        var startPreviousShift = new DateTimeOffset(2025, 6, 20, 2, 0, 0, TimeSpan.FromHours(2));
        var endPreviousShift = new DateTimeOffset(2025, 6, 20, 6, 0, 0, TimeSpan.FromHours(2));
        var previousShift = CreateShift(Guid.NewGuid(), lastShift: false, new ShiftSchedule(startPreviousShift, endPreviousShift));
        var lastItemEndDateOnLine = new DateTimeOffset(2025, 6, 20, 4, 0, 0, TimeSpan.FromHours(2));
        var funcReturnPreviousShift = CreateNextShiftFunc(previousShift);
        var lastItemOnPreviousShift = GenerateLastItemPerView(lastItemEndDateOnLine, previousShift.Id);
        
        var startCurrentShift = new DateTimeOffset(2025, 6, 20, 6, 0, 0, TimeSpan.FromHours(2));
        var endCurrentShift = new DateTimeOffset(2025, 6, 20, 18, 0, 0, TimeSpan.FromHours(2));
        var currentShift = CreateShift(Guid.NewGuid(), lastShift: true, new ShiftSchedule(startCurrentShift, endCurrentShift));
        
        
        var kitStartDate = new DateTime(2025, 6, 20, 8, 0, 0);
        var kitEndDate = new DateTime(2025, 6, 20, 11, 0, 0);
        var optimalKitDuration = new TimeSpan(3, 0, 0);
        const int adjustmentTimInMinutes = 60;
        
        var plannedLineItems = new List<LineItemView>
        {
            GenerateLineItemView(
                new DateTime(2025, 6, 20, 5, 0, 0),
                new DateTime(2025, 6, 20, 6, 0, 0)),
            GenerateLineItemView(
                new DateTime(2025, 6, 20, 9, 0, 0),
                new DateTime(2025, 6, 20, 10, 0, 0))
        };
        
        // Act
        var result = 
            await LineItemQueueBuilder.BuildAsync(currentShift, kitStartDate, kitEndDate, optimalKitDuration, lastItemOnPreviousShift, adjustmentTimInMinutes, plannedLineItems.ToArray(), funcReturnPreviousShift);

        result.LineItemsToCreateOrUpdate.Count.ShouldBe(7);
        
        result.LineItemsToCreateOrUpdate[0].Type.ShouldBe(LineItemType.Downtime);
        result.LineItemsToCreateOrUpdate[0].StartDate.ShouldBe(new DateTime(2025, 6, 20, 4, 0, 0));
        result.LineItemsToCreateOrUpdate[0].ShiftId.ShouldBe(previousShift.Id);
        
        result.LineItemsToCreateOrUpdate[1].Type.ShouldBe(LineItemType.Break);
        result.LineItemsToCreateOrUpdate[1].StartDate.ShouldBe(new DateTime(2025, 6, 20, 5, 0, 0));
        result.LineItemsToCreateOrUpdate[1].ShiftId.ShouldBe(previousShift.Id);
        
        result.LineItemsToCreateOrUpdate[2].Type.ShouldBe(LineItemType.Downtime);
        result.LineItemsToCreateOrUpdate[2].StartDate.ShouldBe(new DateTime(2025, 6, 20, 6, 0, 0));
        result.LineItemsToCreateOrUpdate[2].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemsToCreateOrUpdate[3].Type.ShouldBe(LineItemType.Adjustment);
        result.LineItemsToCreateOrUpdate[3].StartDate.ShouldBe(new DateTime(2025, 6, 20, 7, 0, 0));
        result.LineItemsToCreateOrUpdate[3].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemsToCreateOrUpdate[4].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[4].StartDate.ShouldBe(new DateTime(2025, 6, 20, 8, 0, 0));
        result.LineItemsToCreateOrUpdate[4].EndDate.ShouldBe(new DateTime(2025, 6, 20, 9, 0, 0));
        result.LineItemsToCreateOrUpdate[4].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemsToCreateOrUpdate[5].Type.ShouldBe(LineItemType.Break);
        result.LineItemsToCreateOrUpdate[5].StartDate.ShouldBe(new DateTime(2025, 6, 20, 9, 0, 0));
        result.LineItemsToCreateOrUpdate[5].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemsToCreateOrUpdate[6].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[6].StartDate.ShouldBe(new DateTime(2025, 6, 20, 10, 0, 0));
        result.LineItemsToCreateOrUpdate[6].EndDate.ShouldBe(new DateTime(2025, 6, 20, 11, 0, 0));
        result.LineItemsToCreateOrUpdate[6].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemToShorten.ShouldBeNull();
    }
    
    [Fact]
    public async Task BuildAsync_FirstKitInShiftOnLine_FollowingShiftsWithBreakInPreviousShift2_ReturnsQueueInCorrectOrder()
    {
        
        var startPreviousShift = new DateTimeOffset(2025, 6, 20, 2, 0, 0, TimeSpan.FromHours(2));
        var endPreviousShift = new DateTimeOffset(2025, 6, 20, 6, 0, 0, TimeSpan.FromHours(2));
        var previousShift = CreateShift(Guid.NewGuid(), lastShift: false, new ShiftSchedule(startPreviousShift, endPreviousShift));
        var lastItemEndDateOnLine = new DateTimeOffset(2025, 6, 20, 3, 0, 0, TimeSpan.FromHours(2));
        var funcReturnPreviousShift = CreateNextShiftFunc(previousShift);
        var lastItemOnPreviousShift = GenerateLastItemPerView(lastItemEndDateOnLine, previousShift.Id);
        
        var startCurrentShift = new DateTimeOffset(2025, 6, 20, 6, 0, 0, TimeSpan.FromHours(2));
        var endCurrentShift = new DateTimeOffset(2025, 6, 20, 18, 0, 0, TimeSpan.FromHours(2));
        var currentShift = CreateShift(Guid.NewGuid(), lastShift: true, new ShiftSchedule(startCurrentShift, endCurrentShift));
        
        
        var kitStartDate = new DateTime(2025, 6, 20, 8, 0, 0);
        var kitEndDate = new DateTime(2025, 6, 20, 11, 0, 0);
        var optimalKitDuration = new TimeSpan(3, 0, 0);
        const int adjustmentTimInMinutes = 60;
        
        var plannedLineItems = new List<LineItemView>
        {
            GenerateLineItemView(
                new DateTime(2025, 6, 20, 4, 0, 0),
                new DateTime(2025, 6, 20, 5, 0, 0)),
            GenerateLineItemView(
                new DateTime(2025, 6, 20, 9, 0, 0),
                new DateTime(2025, 6, 20, 10, 0, 0))
        };
        
        // Act
        var result = 
            await LineItemQueueBuilder.BuildAsync(currentShift, kitStartDate, kitEndDate, optimalKitDuration, lastItemOnPreviousShift, adjustmentTimInMinutes, plannedLineItems.ToArray(), funcReturnPreviousShift);

        result.LineItemsToCreateOrUpdate.Count.ShouldBe(8);
        
        result.LineItemsToCreateOrUpdate[0].Type.ShouldBe(LineItemType.Downtime);
        result.LineItemsToCreateOrUpdate[0].StartDate.ShouldBe(new DateTime(2025, 6, 20, 3, 0, 0));
        result.LineItemsToCreateOrUpdate[0].ShiftId.ShouldBe(previousShift.Id);
        
        result.LineItemsToCreateOrUpdate[1].Type.ShouldBe(LineItemType.Break);
        result.LineItemsToCreateOrUpdate[1].StartDate.ShouldBe(new DateTime(2025, 6, 20, 4, 0, 0));
        result.LineItemsToCreateOrUpdate[1].ShiftId.ShouldBe(previousShift.Id);
        
        result.LineItemsToCreateOrUpdate[2].Type.ShouldBe(LineItemType.Downtime);
        result.LineItemsToCreateOrUpdate[2].StartDate.ShouldBe(new DateTime(2025, 6, 20, 5, 0, 0));
        result.LineItemsToCreateOrUpdate[2].ShiftId.ShouldBe(previousShift.Id);
        
        result.LineItemsToCreateOrUpdate[3].Type.ShouldBe(LineItemType.Downtime);
        result.LineItemsToCreateOrUpdate[3].StartDate.ShouldBe(new DateTime(2025, 6, 20, 6, 0, 0));
        result.LineItemsToCreateOrUpdate[3].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemsToCreateOrUpdate[4].Type.ShouldBe(LineItemType.Adjustment);
        result.LineItemsToCreateOrUpdate[4].StartDate.ShouldBe(new DateTime(2025, 6, 20, 7, 0, 0));
        result.LineItemsToCreateOrUpdate[4].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemsToCreateOrUpdate[5].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[5].StartDate.ShouldBe(new DateTime(2025, 6, 20, 8, 0, 0));
        result.LineItemsToCreateOrUpdate[5].EndDate.ShouldBe(new DateTime(2025, 6, 20, 9, 0, 0));
        result.LineItemsToCreateOrUpdate[5].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemsToCreateOrUpdate[6].Type.ShouldBe(LineItemType.Break);
        result.LineItemsToCreateOrUpdate[6].StartDate.ShouldBe(new DateTime(2025, 6, 20, 9, 0, 0));
        result.LineItemsToCreateOrUpdate[6].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemsToCreateOrUpdate[7].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[7].StartDate.ShouldBe(new DateTime(2025, 6, 20, 10, 0, 0));
        result.LineItemsToCreateOrUpdate[7].EndDate.ShouldBe(new DateTime(2025, 6, 20, 11, 0, 0));
        result.LineItemsToCreateOrUpdate[7].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemToShorten.ShouldBeNull();
    }
    
    [Fact]
    public async Task BuildAsync_FirstKitInShiftOnLine_NotFollowingShifts_ReturnsQueueInCorrectOrder()
    {
        var startPreviousShift = new DateTimeOffset(2025, 6, 19, 2, 0, 0, TimeSpan.FromHours(2));
        var endPreviousShift = new DateTimeOffset(2025, 6, 19, 6, 0, 0, TimeSpan.FromHours(2));
        var previousShift = CreateShift(Guid.NewGuid(), lastShift: false, new ShiftSchedule(startPreviousShift, endPreviousShift));
        var lastItemEndDateOnLine = new DateTime(2025, 6, 19, 5, 0, 0);
        var funcReturnPreviousShift = CreateNextShiftFunc(previousShift);
        var lastItemOnPreviousShift = GenerateLastItemPerView(lastItemEndDateOnLine, previousShift.Id);
        
        var startCurrentShift = new DateTimeOffset(2025, 6, 20, 6, 0, 0, TimeSpan.FromHours(2));
        var endCurrentShift = new DateTimeOffset(2025, 6, 20, 18, 0, 0, TimeSpan.FromHours(2));
        var currentShift = CreateShift(Guid.NewGuid(), lastShift: true, new ShiftSchedule(startCurrentShift, endCurrentShift));
        
        
        var kitStartDate = new DateTime(2025, 6, 20, 8, 0, 0);
        var kitEndDate = new DateTime(2025, 6, 20, 11, 0, 0);
        var optimalKitDuration = new TimeSpan(3, 0, 0);
        const int adjustmentTimInMinutes = 60;
        
        var plannedLineItems = new List<LineItemView>
        {
            GenerateLineItemView(
                new DateTime(2025, 6, 20, 9, 0, 0),
                new DateTime(2025, 6, 20, 10, 0, 0))
        };
        
        // Act
        var result = 
            await LineItemQueueBuilder.BuildAsync(currentShift, kitStartDate, kitEndDate, optimalKitDuration, lastItemOnPreviousShift, adjustmentTimInMinutes, plannedLineItems.ToArray(), funcReturnPreviousShift);

        result.LineItemsToCreateOrUpdate.Count.ShouldBe(6);
        
        result.LineItemsToCreateOrUpdate[0].Type.ShouldBe(LineItemType.Downtime);
        result.LineItemsToCreateOrUpdate[0].StartDate.ShouldBe(new DateTime(2025, 6, 19, 5, 0, 0));
        result.LineItemsToCreateOrUpdate[0].ShiftId.ShouldBe(previousShift.Id);
        
        result.LineItemsToCreateOrUpdate[1].Type.ShouldBe(LineItemType.Downtime);
        result.LineItemsToCreateOrUpdate[1].StartDate.ShouldBe(new DateTime(2025, 6, 20, 6, 0, 0));
        result.LineItemsToCreateOrUpdate[1].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemsToCreateOrUpdate[2].Type.ShouldBe(LineItemType.Adjustment);
        result.LineItemsToCreateOrUpdate[2].StartDate.ShouldBe(new DateTime(2025, 6, 20, 7, 0, 0));
        result.LineItemsToCreateOrUpdate[2].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemsToCreateOrUpdate[3].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[3].StartDate.ShouldBe(new DateTime(2025, 6, 20, 8, 0, 0));
        result.LineItemsToCreateOrUpdate[3].EndDate.ShouldBe(new DateTime(2025, 6, 20, 9, 0, 0));
        result.LineItemsToCreateOrUpdate[3].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemsToCreateOrUpdate[4].Type.ShouldBe(LineItemType.Break);
        result.LineItemsToCreateOrUpdate[4].StartDate.ShouldBe(new DateTime(2025, 6, 20, 9, 0, 0));
        result.LineItemsToCreateOrUpdate[4].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemsToCreateOrUpdate[5].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[5].StartDate.ShouldBe(new DateTime(2025, 6, 20, 10, 0, 0));
        result.LineItemsToCreateOrUpdate[5].EndDate.ShouldBe(new DateTime(2025, 6, 20, 11, 0, 0));
        result.LineItemsToCreateOrUpdate[5].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemToShorten.ShouldBeNull();
    }
    
    [Fact]
    public async Task BuildAsync_FirstKitInShiftOnLine_NotFollowingShiftsWithBreakInPreviousShift_ReturnsQueueInCorrectOrder()
    {
        
        var startPreviousShift = new DateTimeOffset(2025, 6, 19, 2, 0, 0, TimeSpan.FromHours(2));
        var endPreviousShift = new DateTimeOffset(2025, 6, 19, 6, 0, 0, TimeSpan.FromHours(2));
        var previousShift = CreateShift(Guid.NewGuid(), lastShift: false, new ShiftSchedule(startPreviousShift, endPreviousShift));
        var lastItemEndDateOnLine = new DateTimeOffset(2025, 6, 19, 3, 0, 0, TimeSpan.FromHours(2));
        var funcReturnPreviousShift = CreateNextShiftFunc(previousShift);
        var lastItemOnPreviousShift = GenerateLastItemPerView(lastItemEndDateOnLine, previousShift.Id);
        
        var startCurrentShift = new DateTimeOffset(2025, 6, 20, 6, 0, 0, TimeSpan.FromHours(2));
        var endCurrentShift = new DateTimeOffset(2025, 6, 20, 18, 0, 0, TimeSpan.FromHours(2));
        var currentShift = CreateShift(Guid.NewGuid(), lastShift: true, new ShiftSchedule(startCurrentShift, endCurrentShift));
        
        
        var kitStartDate = new DateTime(2025, 6, 20, 8, 0, 0);
        var kitEndDate = new DateTime(2025, 6, 20, 11, 0, 0);
        var optimalKitDuration = new TimeSpan(3, 0, 0);
        const int adjustmentTimInMinutes = 60;
        
        var plannedLineItems = new List<LineItemView>
        {
            GenerateLineItemView(
                new DateTime(2025, 6, 19, 4, 0, 0),
                new DateTime(2025, 6, 19, 5, 0, 0)),
            GenerateLineItemView(
                new DateTime(2025, 6, 20, 9, 0, 0),
                new DateTime(2025, 6, 20, 10, 0, 0))
        };
        
        // Act
        var result = 
            await LineItemQueueBuilder.BuildAsync(currentShift, kitStartDate, kitEndDate, optimalKitDuration, lastItemOnPreviousShift, adjustmentTimInMinutes, plannedLineItems.ToArray(), funcReturnPreviousShift);

        result.LineItemsToCreateOrUpdate.Count.ShouldBe(8);
        
        result.LineItemsToCreateOrUpdate[0].Type.ShouldBe(LineItemType.Downtime);
        result.LineItemsToCreateOrUpdate[0].StartDate.ShouldBe(new DateTime(2025, 6, 19, 3, 0, 0));
        result.LineItemsToCreateOrUpdate[0].ShiftId.ShouldBe(previousShift.Id);
        
        result.LineItemsToCreateOrUpdate[1].Type.ShouldBe(LineItemType.Break);
        result.LineItemsToCreateOrUpdate[1].StartDate.ShouldBe(new DateTime(2025, 6, 19, 4, 0, 0));
        result.LineItemsToCreateOrUpdate[1].ShiftId.ShouldBe(previousShift.Id);
        
        result.LineItemsToCreateOrUpdate[2].Type.ShouldBe(LineItemType.Downtime);
        result.LineItemsToCreateOrUpdate[2].StartDate.ShouldBe(new DateTime(2025, 6, 19, 5, 0, 0));
        result.LineItemsToCreateOrUpdate[2].ShiftId.ShouldBe(previousShift.Id);
        
        result.LineItemsToCreateOrUpdate[3].Type.ShouldBe(LineItemType.Downtime);
        result.LineItemsToCreateOrUpdate[3].StartDate.ShouldBe(new DateTime(2025, 6, 20, 6, 0, 0));
        result.LineItemsToCreateOrUpdate[3].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemsToCreateOrUpdate[4].Type.ShouldBe(LineItemType.Adjustment);
        result.LineItemsToCreateOrUpdate[4].StartDate.ShouldBe(new DateTime(2025, 6, 20, 7, 0, 0));
        result.LineItemsToCreateOrUpdate[4].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemsToCreateOrUpdate[5].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[5].StartDate.ShouldBe(new DateTime(2025, 6, 20, 8, 0, 0));
        result.LineItemsToCreateOrUpdate[5].EndDate.ShouldBe(new DateTime(2025, 6, 20, 9, 0, 0));
        result.LineItemsToCreateOrUpdate[5].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemsToCreateOrUpdate[6].Type.ShouldBe(LineItemType.Break);
        result.LineItemsToCreateOrUpdate[6].StartDate.ShouldBe(new DateTime(2025, 6, 20, 9, 0, 0));
        result.LineItemsToCreateOrUpdate[6].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemsToCreateOrUpdate[7].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[7].StartDate.ShouldBe(new DateTime(2025, 6, 20, 10, 0, 0));
        result.LineItemsToCreateOrUpdate[7].EndDate.ShouldBe(new DateTime(2025, 6, 20, 11, 0, 0));
        result.LineItemsToCreateOrUpdate[7].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemToShorten.ShouldBeNull();
    }
    
    [Fact]
    public async Task BuildAsync_FirstKitInShiftOnLine_FollowingShifts_ReturnsQueueInCorrectOrderWithOptimalTime()
    {
        var startPreviousShift = new DateTimeOffset(2025, 6, 20, 2, 0, 0, TimeSpan.FromHours(2));
        var endPreviousShift = new DateTimeOffset(2025, 6, 20, 6, 0, 0, TimeSpan.FromHours(2));
        var previousShift = CreateShift(Guid.NewGuid(), lastShift: false, new ShiftSchedule(startPreviousShift, endPreviousShift));
        var lastItemEndDateOnLine = new DateTime(2025, 6, 20, 5, 0, 0);
        var funcReturnPreviousShift = CreateNextShiftFunc(previousShift);
        var lastItemOnPreviousShift = GenerateLastItemPerView(lastItemEndDateOnLine, previousShift.Id);
        
        var startCurrentShift = new DateTimeOffset(2025, 6, 20, 6, 0, 0, TimeSpan.FromHours(2));
        var endCurrentShift = new DateTimeOffset(2025, 6, 20, 18, 0, 0, TimeSpan.FromHours(2));
        var currentShift = CreateShift(Guid.NewGuid(), lastShift: true, new ShiftSchedule(startCurrentShift, endCurrentShift));
        
        
        var kitStartDate = new DateTime(2025, 6, 20, 5, 0, 0);
        var kitEndDate = new DateTime(2025, 6, 20, 7, 0, 0);
        var optimalKitDuration = new TimeSpan(0, 10, 0);
        const int adjustmentTimInMinutes = 15;

        // Act
        var result = 
            await LineItemQueueBuilder.BuildAsync(currentShift, kitStartDate, kitEndDate, optimalKitDuration, 
                lastItemOnPreviousShift, adjustmentTimInMinutes, [], funcReturnPreviousShift);

        result.LineItemsToCreateOrUpdate.Count.ShouldBe(4);
        
        result.LineItemsToCreateOrUpdate[0].Type.ShouldBe(LineItemType.Downtime);
        result.LineItemsToCreateOrUpdate[0].StartDate.ShouldBe(new DateTime(2025, 6, 20, 5, 0, 0));
        result.LineItemsToCreateOrUpdate[0].ShiftId.ShouldBe(previousShift.Id);
        
        result.LineItemsToCreateOrUpdate[1].Type.ShouldBe(LineItemType.Downtime);
        result.LineItemsToCreateOrUpdate[1].StartDate.ShouldBe(new DateTime(2025, 6, 20, 6, 0, 0));
        result.LineItemsToCreateOrUpdate[1].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemsToCreateOrUpdate[2].Type.ShouldBe(LineItemType.Adjustment);
        result.LineItemsToCreateOrUpdate[2].StartDate.ShouldBe(new DateTime(2025, 6, 20, 6, 35, 0));
        result.LineItemsToCreateOrUpdate[2].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemsToCreateOrUpdate[3].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[3].StartDate.ShouldBe(new DateTime(2025, 6, 20, 6, 50, 0));
        result.LineItemsToCreateOrUpdate[3].EndDate.ShouldBe(new DateTime(2025, 6, 20, 7, 0, 0));
        result.LineItemsToCreateOrUpdate[3].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemToShorten.ShouldBeNull();
    }
    
    [Fact]
    public async Task BuildAsync_FirstKitInShiftOnLine_FollowingShifts_ReturnsQueueInCorrectOrderWithShortenOptimalTime_ToNotOverlapWithLastKitOnPreviousShift()
    {
        var startPreviousShift = new DateTimeOffset(2025, 6, 20, 2, 0, 0, TimeSpan.FromHours(2));
        var endPreviousShift = new DateTimeOffset(2025, 6, 20, 6, 0, 0, TimeSpan.FromHours(2));
        var previousShift = CreateShift(Guid.NewGuid(), lastShift: false, new ShiftSchedule(startPreviousShift, endPreviousShift));
        var lastItemEndDateOnLine = new DateTime(2025, 6, 20, 5, 55, 0);
        var funcReturnPreviousShift = CreateNextShiftFunc(previousShift);
        var lastItemOnPreviousShift = GenerateLastItemPerView(lastItemEndDateOnLine, previousShift.Id);
        
        var startCurrentShift = new DateTimeOffset(2025, 6, 20, 6, 0, 0, TimeSpan.FromHours(2));
        var endCurrentShift = new DateTimeOffset(2025, 6, 20, 18, 0, 0, TimeSpan.FromHours(2));
        var currentShift = CreateShift(Guid.NewGuid(), lastShift: true, new ShiftSchedule(startCurrentShift, endCurrentShift));
        
        
        var kitStartDate = new DateTime(2025, 6, 20, 5, 53, 0);
        var kitEndDate = new DateTime(2025, 6, 20, 6, 3, 0);
        var optimalKitDuration = new TimeSpan(0, 10, 0);
        const int adjustmentTimInMinutes = 15;

        // Act
        var result = 
            await LineItemQueueBuilder.BuildAsync(currentShift, kitStartDate, kitEndDate, optimalKitDuration, 
                lastItemOnPreviousShift, adjustmentTimInMinutes, [], funcReturnPreviousShift);

        result.LineItemsToCreateOrUpdate.Count.ShouldBe(1);
        
        result.LineItemsToCreateOrUpdate[0].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[0].StartDate.ShouldBe(new DateTime(2025, 6, 20, 5, 55, 0));
        result.LineItemsToCreateOrUpdate[0].EndDate.ShouldBe(new DateTime(2025, 6, 20, 6, 3, 0));
        result.LineItemsToCreateOrUpdate[0].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemToShorten.ShouldBeNull();
    }
    
    [Fact]
    public async Task BuildAsync_FirstKitInShiftOnLine_FollowingShifts_ReturnsQueueInCorrectOrder_WithFirstKitBeforeShiftStart()
    {
        var startPreviousShift = new DateTimeOffset(2025, 6, 20, 2, 0, 0, TimeSpan.FromHours(2));
        var endPreviousShift = new DateTimeOffset(2025, 6, 20, 6, 0, 0, TimeSpan.FromHours(2));
        var previousShift = CreateShift(Guid.NewGuid(), lastShift: false, new ShiftSchedule(startPreviousShift, endPreviousShift));
        var lastItemEndDateOnLine = new DateTime(2025, 6, 20, 5, 50, 0);
        var funcReturnPreviousShift = CreateNextShiftFunc(previousShift);
        var lastItemOnPreviousShift = GenerateLastItemPerView(lastItemEndDateOnLine, previousShift.Id);
        
        var startCurrentShift = new DateTimeOffset(2025, 6, 20, 6, 0, 0, TimeSpan.FromHours(2));
        var endCurrentShift = new DateTimeOffset(2025, 6, 20, 18, 0, 0, TimeSpan.FromHours(2));
        var currentShift = CreateShift(Guid.NewGuid(), lastShift: true, new ShiftSchedule(startCurrentShift, endCurrentShift));
        
        
        var kitStartDate = new DateTime(2025, 6, 20, 5, 50, 0);
        var kitEndDate = new DateTime(2025, 6, 20, 6, 5, 0);
        var optimalKitDuration = new TimeSpan(0, 10, 0);
        const int adjustmentTimInMinutes = 15;

        // Act
        var result = 
            await LineItemQueueBuilder.BuildAsync(currentShift, kitStartDate, kitEndDate, optimalKitDuration, 
                lastItemOnPreviousShift, adjustmentTimInMinutes, [], funcReturnPreviousShift);

        result.LineItemsToCreateOrUpdate.Count.ShouldBe(2);
        
        result.LineItemsToCreateOrUpdate[0].Type.ShouldBe(LineItemType.Downtime);
        result.LineItemsToCreateOrUpdate[0].StartDate.ShouldBe(new DateTime(2025, 6, 20, 5, 50, 0));
        result.LineItemsToCreateOrUpdate[0].EndDate.ShouldBe(new DateTime(2025, 6, 20, 5, 55, 0));
        result.LineItemsToCreateOrUpdate[0].ShiftId.ShouldBe(previousShift.Id);
        
        result.LineItemsToCreateOrUpdate[1].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[1].StartDate.ShouldBe(new DateTime(2025, 6, 20, 5, 55, 0));
        result.LineItemsToCreateOrUpdate[1].EndDate.ShouldBe(new DateTime(2025, 6, 20, 6, 5, 0));
        result.LineItemsToCreateOrUpdate[1].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemToShorten.ShouldBeNull();
    }
    
    [Fact]
    public async Task BuildAsync_FirstKitInShiftOnLine_NotFollowingShifts_ReturnsQueueInCorrectOrder_WithFirstKitStartsBeforeShift()
    {
        var startPreviousShift = new DateTimeOffset(2025, 6, 19, 2, 0, 0, TimeSpan.FromHours(2));
        var endPreviousShift = new DateTimeOffset(2025, 6, 19, 6, 0, 0, TimeSpan.FromHours(2));
        var previousShift = CreateShift(Guid.NewGuid(), lastShift: false, new ShiftSchedule(startPreviousShift, endPreviousShift));
        var lastItemEndDateOnLine = new DateTime(2025, 6, 19, 5, 40, 0);
        var funcReturnPreviousShift = CreateNextShiftFunc(previousShift);
        var lastItemOnPreviousShift = GenerateLastItemPerView(lastItemEndDateOnLine, previousShift.Id);
        
        var startCurrentShift = new DateTimeOffset(2025, 6, 20, 6, 0, 0, TimeSpan.FromHours(2));
        var endCurrentShift = new DateTimeOffset(2025, 6, 20, 18, 0, 0, TimeSpan.FromHours(2));
        var currentShift = CreateShift(Guid.NewGuid(), lastShift: true, new ShiftSchedule(startCurrentShift, endCurrentShift));
        
        
        var kitStartDate = new DateTime(2025, 6, 20, 4, 50, 0);
        var kitEndDate = new DateTime(2025, 6, 20, 5, 5, 0);
        var optimalKitDuration = new TimeSpan(0, 10, 0);
        const int adjustmentTimInMinutes = 15;

        // Act
        var result = 
            await LineItemQueueBuilder.BuildAsync(currentShift, kitStartDate, kitEndDate, optimalKitDuration, 
                lastItemOnPreviousShift, adjustmentTimInMinutes, [], funcReturnPreviousShift);

        result.LineItemsToCreateOrUpdate.Count.ShouldBe(2);
        
        result.LineItemsToCreateOrUpdate[0].Type.ShouldBe(LineItemType.Downtime);
        result.LineItemsToCreateOrUpdate[0].StartDate.ShouldBe(new DateTime(2025, 6, 19, 5, 40, 0));
        result.LineItemsToCreateOrUpdate[0].EndDate.ShouldBe(new DateTime(2025, 6, 19, 6, 0, 0));
        result.LineItemsToCreateOrUpdate[0].ShiftId.ShouldBe(previousShift.Id);
        
        result.LineItemsToCreateOrUpdate[1].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[1].StartDate.ShouldBe(new DateTime(2025, 6, 20, 4, 55, 0));
        result.LineItemsToCreateOrUpdate[1].EndDate.ShouldBe(new DateTime(2025, 6, 20, 5, 5, 0));
        result.LineItemsToCreateOrUpdate[1].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemToShorten.ShouldBeNull();
    }
    
    [Fact]
    public async Task BuildAsync_LastItemOnLineInShiftEvenBeforeNextToLastShift_BuildsCorrectlyFromNextToLastShiftStartDate()
    {
        var lastItemEndDateOnLine = new DateTime(2025, 6, 18, 5, 40, 0);
        var lastItemOnPastShift = GenerateLastItemPerView(lastItemEndDateOnLine);
        
        var startPreviousShift = new DateTimeOffset(2025, 6, 19, 2, 0, 0, TimeSpan.FromHours(2));
        var endPreviousShift = new DateTimeOffset(2025, 6, 19, 6, 0, 0, TimeSpan.FromHours(2));
        var previousShift = CreateShift(Guid.NewGuid(), lastShift: false, new ShiftSchedule(startPreviousShift, endPreviousShift));
        
        var funcReturnPreviousShift = CreateNextShiftFunc(previousShift);
        
        var startCurrentShift = new DateTimeOffset(2025, 6, 20, 6, 0, 0, TimeSpan.FromHours(2));
        var endCurrentShift = new DateTimeOffset(2025, 6, 20, 18, 0, 0, TimeSpan.FromHours(2));
        var currentShift = CreateShift(Guid.NewGuid(), lastShift: true, new ShiftSchedule(startCurrentShift, endCurrentShift));
        
        var kitStartDate = new DateTime(2025, 6, 20, 6, 40, 0);
        var kitEndDate = new DateTime(2025, 6, 20, 6, 50, 0);
        var optimalKitDuration = new TimeSpan(0, 10, 0);
        const int adjustmentTimInMinutes = 15;

        // Act
        var result = 
            await LineItemQueueBuilder.BuildAsync(currentShift, kitStartDate, kitEndDate, optimalKitDuration, 
                lastItemOnPastShift, adjustmentTimInMinutes, [], funcReturnPreviousShift);

        result.LineItemsToCreateOrUpdate.Count.ShouldBe(4);
        
        result.LineItemsToCreateOrUpdate[0].Type.ShouldBe(LineItemType.Downtime);
        result.LineItemsToCreateOrUpdate[0].StartDate.ShouldBe(new DateTime(2025, 6, 19, 2, 0, 0));
        result.LineItemsToCreateOrUpdate[0].EndDate.ShouldBe(new DateTime(2025, 6, 19, 6, 0, 0));
        result.LineItemsToCreateOrUpdate[0].ShiftId.ShouldBe(previousShift.Id);
        
        result.LineItemsToCreateOrUpdate[1].Type.ShouldBe(LineItemType.Downtime);
        result.LineItemsToCreateOrUpdate[1].StartDate.ShouldBe(new DateTime(2025, 6, 20, 6, 0, 0));
        result.LineItemsToCreateOrUpdate[1].EndDate.ShouldBe(new DateTime(2025, 6, 20, 6, 25, 0));
        result.LineItemsToCreateOrUpdate[1].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemsToCreateOrUpdate[2].Type.ShouldBe(LineItemType.Adjustment);
        result.LineItemsToCreateOrUpdate[2].StartDate.ShouldBe(new DateTime(2025, 6, 20, 6, 25, 0));
        result.LineItemsToCreateOrUpdate[2].EndDate.ShouldBe(new DateTime(2025, 6, 20, 6, 40, 0));
        result.LineItemsToCreateOrUpdate[2].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemsToCreateOrUpdate[3].Type.ShouldBe(LineItemType.Kit);
        result.LineItemsToCreateOrUpdate[3].StartDate.ShouldBe(new DateTime(2025, 6, 20, 6, 40, 0));
        result.LineItemsToCreateOrUpdate[3].EndDate.ShouldBe(new DateTime(2025, 6, 20, 6, 50, 0));
        result.LineItemsToCreateOrUpdate[3].ShiftId.ShouldBe(currentShift.Id);
        
        result.LineItemToShorten.ShouldBeNull();
    }
    
    private static Shift CreateShift(Guid? id = null, bool lastShift = true, ShiftSchedule? shiftSchedule = null)
    {
        return new Shift(
            Guid.NewGuid().ToString(), 
            lastShift, 
            !lastShift, 
            shiftSchedule ?? new ShiftSchedule(DateTimeOffset.UtcNow.AddHours(-2), DateTimeOffset.UtcNow.AddHours(6)),
            DateTimeOffset.UtcNow,
            [],
            id ?? Guid.NewGuid());
    }
    
    private static Func<Task<ErrorOr<Shift>>> CreateNextShiftFunc(Shift? shift = null)
    {
        return () => Task.FromResult<ErrorOr<Shift>>(shift ?? CreateShift());
    }
    
    private static LastItemPerLineView GenerateLastItemPerView(DateTimeOffset lastFinishedDate, Guid? shiftId = null)
    {
        return new LastItemPerLineView
        {
            Id = Guid.NewGuid().ToString(),
            ItemId = Guid.NewGuid(),
            ShiftId = shiftId ?? Guid.NewGuid(),
            LastFinishedDate = lastFinishedDate
        };
    }
    
    private static LineItemView GenerateLineItemView(DateTime startDate, DateTime endDate)
    {
        return new LineItemView
        {
            StartDate = startDate,
            EndDate = endDate,
            Type = LineItemType.Break,
            Id = Guid.NewGuid()
        };
    }
}