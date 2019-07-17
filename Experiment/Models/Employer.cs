using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experiment.Models
{
    public class Employer: INotifyPropertyChanged
    {
        private string _firstName;
        private string _lastName;
        private string _phoneNumber;

        public Employer()
        {
           
        }

        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string FirstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                _firstName = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("FirstName"));
            }
        }

        public string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                _lastName = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("LastName"));
            }
        }

        public string PhoneNumber
        {
            get
            {

                return formatPhoneNumber(_phoneNumber);
            }
            set
            {
                _phoneNumber = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("PhoneNumber"));
            }
        }

        public Employer DeepCopy()
        {
            Employer copiedEmployer = (Employer)this.MemberwiseClone();
            return copiedEmployer;
        }

        private string formatPhoneNumber(string phoneNumber)
        {
            return String.Format("{0} {1} {2} {3} {4}",
                phoneNumber.Substring(0, 2),
                phoneNumber.Substring(2, 2),
                phoneNumber.Substring(4, 2),
                phoneNumber.Substring(6, 2),
                phoneNumber.Substring(8, 2)
                );
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
