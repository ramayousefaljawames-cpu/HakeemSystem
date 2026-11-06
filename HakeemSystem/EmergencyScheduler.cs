namespace HakeemSystem;

public sealed class EmergencyScheduler
{
    private readonly List<Doctor> _doctors = [];
    private readonly List<Patient> _registeredPatients = [];
    private readonly List<MedicalVisit> _queue = [];
    private readonly List<MedicalVisit> _completedVisits = [];

    public IReadOnlyList<Doctor> Doctors => _doctors;
    public IReadOnlyList<Patient> Patients => _registeredPatients;
    public IReadOnlyList<MedicalVisit> Queue => _queue.OrderByDescending(v => v.Priority).ThenBy(v => v.CreatedAt).ToList();
    public IReadOnlyList<MedicalVisit> CompletedVisits => _completedVisits;

    public void AddDoctor(Doctor doctor)
    {
        ArgumentNullException.ThrowIfNull(doctor);
        _doctors.Add(doctor);
    }

    public Patient RegisterPatient(string fullName, int age, InsuranceType insuranceType, bool hasBleeding, int heartRate, int bloodPressure, int temperature, string symptoms)
    {
        var patient = new Patient(fullName, age, insuranceType, hasBleeding, heartRate, bloodPressure, temperature, symptoms);
        _registeredPatients.Add(patient);
        return patient;
    }

    public Patient RegisterPatient(Patient patient)
    {
        ArgumentNullException.ThrowIfNull(patient);
        _registeredPatients.Add(patient);
        return patient;
    }

    public static int CalculatePriority(bool bleeding, int heartRate)
    {
        return bleeding || heartRate > 120 ? 1 : 0;
    }

    public static PriorityLevel CalculatePriority(Patient patient)
    {
        ArgumentNullException.ThrowIfNull(patient);

        if (patient.HasBleeding || patient.HeartRate > 120 || patient.ClinicalStatus == ClinicalStatus.Critical)
        {
            return PriorityLevel.Critical;
        }

        if (patient.HeartRate > 120 || patient.BloodPressure > 160 || patient.ClinicalStatus == ClinicalStatus.Urgent)
        {
            return PriorityLevel.High;
        }

        if (patient.HeartRate > 90 || patient.BloodPressure > 140)
        {
            return PriorityLevel.Moderate;
        }

        return PriorityLevel.Low;
    }

    public MedicalVisit SchedulePatient(Patient patient)
    {
        ArgumentNullException.ThrowIfNull(patient);

        var priority = CalculatePriority(patient);
        var visit = new MedicalVisit(patient, priority, patient.ClinicalStatus == ClinicalStatus.Deceased ? ClinicalStatus.Deceased : ClinicalStatus.Urgent)
        {
            Notes = patient.Symptoms
        };

        _queue.Add(visit);
        _queue.Sort((left, right) => right.Priority.CompareTo(left.Priority));
        return visit;
    }

    public MedicalVisit? AssignDoctor(Patient patient)
    {
        var visit = _queue.FirstOrDefault(v => v.Patient.PatientId == patient.PatientId);
        if (visit is null)
        {
            return null;
        }

        var doctor = _doctors.FirstOrDefault(d => d.IsAvailable);
        if (doctor is null)
        {
            return null;
        }

        doctor.IsAvailable = false;
        visit.AssignedDoctor = doctor;
        visit.ClinicalStatus = ClinicalStatus.Urgent;
        return visit;
    }

    public bool UpdateQueueAfterTreatment(string patientId)
    {
        var visit = _queue.FirstOrDefault(v => v.Patient.PatientId == patientId);
        if (visit is null)
        {
            return false;
        }

        _queue.Remove(visit);
        _completedVisits.Add(visit);
        if (visit.AssignedDoctor is not null)
        {
            visit.AssignedDoctor.IsAvailable = true;
        }

        return true;
    }

    public bool UpdateQueueAfterTreatment(Patient patient)
    {
        ArgumentNullException.ThrowIfNull(patient);
        return UpdateQueueAfterTreatment(patient.PatientId);
    }

    public Prescription AddPrescription(string patientId, string medicineName, string dosage, string notes)
    {
        var visit = _queue.FirstOrDefault(v => v.Patient.PatientId == patientId) ??
                    _completedVisits.FirstOrDefault(v => v.Patient.PatientId == patientId);

        if (visit is null)
        {
            throw new InvalidOperationException("The patient does not have an active or completed visit.");
        }

        var prescription = new Prescription(medicineName, dosage, notes);
        visit.Prescriptions.Add(prescription);
        return prescription;
    }
}
