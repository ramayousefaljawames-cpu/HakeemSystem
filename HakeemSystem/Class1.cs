namespace HakeemSystem;

public static class HakeemSystemFacade
{
    public static EmergencyScheduler CreateDefaultScheduler()
    {
        var scheduler = new EmergencyScheduler();
        scheduler.AddDoctor(new Doctor("Dr. Samer", "Emergency Medicine"));
        scheduler.AddDoctor(new Doctor("Dr. Leen", "Cardiology"));
        scheduler.AddDoctor(new Doctor("Dr. Omar", "General Surgery"));
        return scheduler;
    }
}
