namespace HakeemSystem;

public enum InsuranceType
{
    Military,
    RoyalMedicalServices,
    CivilGovernment,
    Private
}

public enum PriorityLevel
{
    Low = 0,
    Moderate = 1,
    High = 2,
    Critical = 3
}

public enum ClinicalStatus
{
    Stable,
    Urgent,
    Critical,
    Deceased
}

public sealed class Patient
{
    public string PatientId { get; }
    public string FullName { get; set; } = string.Empty;
    public int Age { get; set; }
    public InsuranceType InsuranceType { get; set; }
    public bool HasBleeding { get; set; }
    public int HeartRate { get; set; }
    public int BloodPressure { get; set; }
    public int Temperature { get; set; }
    public string Symptoms { get; set; } = string.Empty;
    public ClinicalStatus ClinicalStatus { get; set; } = ClinicalStatus.Stable;
    public DateTime RegisteredAt { get; }

    public Patient(string fullName, int age, InsuranceType insuranceType, bool hasBleeding, int heartRate, int bloodPressure, int temperature, string symptoms)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("Patient name is required.", nameof(fullName));
        }

        if (age <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(age), "Age must be greater than zero.");
        }

        if (heartRate <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(heartRate), "Heart rate must be greater than zero.");
        }

        PatientId = Guid.NewGuid().ToString("N")[..8];
        FullName = fullName;
        Age = age;
        InsuranceType = insuranceType;
        HasBleeding = hasBleeding;
        HeartRate = heartRate;
        BloodPressure = bloodPressure;
        Temperature = temperature;
        Symptoms = symptoms;
        RegisteredAt = DateTime.UtcNow;
    }
}

public sealed class Doctor
{
    public string DoctorId { get; }
    public string Name { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;

    public Doctor(string name, string specialty)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Doctor name is required.", nameof(name));
        }

        DoctorId = Guid.NewGuid().ToString("N")[..8];
        Name = name;
        Specialty = specialty;
    }
}

public sealed class Prescription
{
    public string PrescriptionId { get; }
    public string MedicineName { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;

    public Prescription(string medicineName, string dosage, string notes)
    {
        PrescriptionId = Guid.NewGuid().ToString("N")[..8];
        MedicineName = medicineName;
        Dosage = dosage;
        Notes = notes;
    }
}

public sealed class MedicalVisit
{
    public string VisitId { get; }
    public Patient Patient { get; }
    public PriorityLevel Priority { get; set; }
    public ClinicalStatus ClinicalStatus { get; set; }
    public Doctor? AssignedDoctor { get; set; }
    public string Notes { get; set; } = string.Empty;
    public List<Prescription> Prescriptions { get; } = [];
    public DateTime CreatedAt { get; }

    public MedicalVisit(Patient patient, PriorityLevel priority, ClinicalStatus clinicalStatus, string notes = "")
    {
        VisitId = Guid.NewGuid().ToString("N")[..8];
        Patient = patient;
        Priority = priority;
        ClinicalStatus = clinicalStatus;
        Notes = notes;
        CreatedAt = DateTime.UtcNow;
    }
}
