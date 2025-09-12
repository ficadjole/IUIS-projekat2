using MVVM1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Helpers
{
    public class ValidationErrors : BindableBase
     {

        //cuvanje gresaka za svako polje
        private readonly Dictionary<string, string> validationErrors = new Dictionary<string, string>();
        //ppolje za koje se desila greksa, greska koja treba da se ispise

        public bool IsValid
        {
            get { return this.validationErrors.Count < 1; }
        }

        public string this[string fieldName]
        {
            get
            {
                return this.validationErrors.ContainsKey(fieldName) ? this.validationErrors[fieldName] : string.Empty;
            }

            set
            {
                //ako postoji taj kljuc i ako je ono sto se dodaje prazno znaci da je greska ispravljena i sklanja se iz recnika
                if (this.validationErrors.ContainsKey(fieldName))
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        this.validationErrors.Remove(fieldName);
                    }
                    else
                    {
                        this.validationErrors[fieldName] = value; //ako postoji poruka greske da se nova greska prikaze
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        this.validationErrors.Add(fieldName, value); //ako ne postoji greska i nova greska nije prazna da se doda
                    }


                }
                this.OnPropertyChanged("IsValid");
            }



        }
        public void Clear()
        {
            validationErrors.Clear();
        }

    }
}
