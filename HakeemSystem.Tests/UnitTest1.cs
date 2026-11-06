using HakeemSystem;

namespace HakeemSystem.Tests;

public class EmergencySchedulerTests
{
    [Fact]
    public void CalculatePriority_ReturnsHighWhenBleedingOrHighHeartRate()
    {
        Assert.Equal(1, EmergencyScheduler.CalculatePriority(true, 80));
        Assert.Equal(1, EmergencyScheduler.CalculatePriority(false, 130));
        Assert.Equal(0, EmergencyScheduler.CalculatePriority(false, 70));
    }

    [Fact]
    public void RegisterAndSchedulePatient_OrdersByPriority()
    {
        var scheduler = HakeemSystemFacade.CreateDefaultScheduler();
        var critical = scheduler.RegisterPatient("Ali", 30, InsuranceType.Military, true, 140, 90, 38, "Severe bleeding");
        var low = scheduler.RegisterPatient("Sara", 22, InsuranceType.Private, false, 72, 118, 37, "Mild fever");

        var criticalVisit = scheduler.SchedulePatient(critical);
        var lowVisit = scheduler.SchedulePatient(low);

        Assert.Equal(PriorityLevel.Critical, criticalVisit.Priority);
        Assert.Equal(PriorityLevel.Low, lowVisit.Priority);
        Assert.Equal(2, scheduler.Queue.Count);
    }

    [Fact]
    public void AssignDoctor_UsesAvailableDoctorAndTracksQueueUpdate()
    {
        var scheduler = HakeemSystemFacade.CreateDefaultScheduler();
        var patient = scheduler.RegisterPatient("Nour", 41, InsuranceType.RoyalMedicalServices, false, 128, 170, 39, "Chest pain");

        var visit = scheduler.SchedulePatient(patient);
        var result = scheduler.AssignDoctor(patient);

        Assert.NotNull(result);
        Assert.NotNull(result.AssignedDoctor);
        Assert.False(result.AssignedDoctor.IsAvailable);

        Assert.True(scheduler.UpdateQueueAfterTreatment(patient));
        Assert.Equal(1, scheduler.CompletedVisits.Count);
    }
}
