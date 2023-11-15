namespace FifthGroup_front.Models
{
    public class CActiveWrap
    {
        private Application _active;
        public Application active
        {
            get { return _active; }
            set { _active = value; }
        }
        public CActiveWrap()
        {
            _active = new Application();
        }
        public int ApplyCode
        {
            get { return _active.ApplyCode; }
            set { _active.ApplyCode = value; }
        }
        public string ActivityName
        {
            get { return _active.ActivityName; }
            set { _active.ActivityName = value; }
        }

        public string HouseholdCode
        {
            get { return _active.HouseholdCode; }
            set { _active.HouseholdCode = value; }
        }

        public string Name
        {
            get { return _active.Name; }
            set { _active.Name = value; }
        }

        public string Phone
        {
            get { return _active.Phone; }
            set { _active.Phone = value; }
        }

        public string Activities
        {
            get { return _active.Activities; }
            set { _active.Activities = value; }
        }

        public int? ReserveCode
        {
            get { return _active.ReserveCode; }
            set { _active.ReserveCode = value; }
        }
        public DateTime DateStart
        {
            get { return _active.DateStart; }
            set { _active.DateStart = value; }
        }
        public DateTime DateEnd
        {
            get { return _active.DateEnd; }
            set { _active.DateEnd = value; }
        }
        public string Email
        {
            get { return _active.Email; }
            set { _active.Email = value; }
        }
        public int? MaxApplicants
        {
            get { return _active.MaxApplicants; }
            set { _active.MaxApplicants = value; }
        }
        public int? Applicants
        {
            get { return _active.Applicants; }
            set { _active.Applicants = value; }
        }
        public int State
        {
            get { return _active.State; }
            set { _active.State = value; }
        }
        public string? Introduce
        {
            get { return _active.Introduce; }
            set { _active.Introduce = value; }
        }
        public string? Image
        {
            get { return _active.Image; }
            set { _active.Image = value; }
        }
        public IFormFile photo { get; set; }
    }
}
