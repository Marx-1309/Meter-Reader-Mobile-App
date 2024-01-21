

namespace SampleMauiMvvmApp.ModelWrappers
{
    public partial class CustomerWrapper : ObservableObject
    {
        public CustomerWrapper(Customer customerModel)
        {
            if (customerModel != null)
            {
                Custnmbr = customerModel.CUSTNMBR;
                Custname = customerModel.CUSTNAME;
                #region
                Custclas = customerModel.CUSTCLAS;
                State = customerModel.STATE;
                Zip = customerModel.ZIP;
                AreaErf = customerModel.AreaErf;
                ModelTitle = customerModel.ModelTitle;
                #endregion
                Readings = new();
                customerModel.Readings?.ForEach(x =>
                    Readings.Add(new ReadingWrapper(x))
                );
            }
        }

        public CustomerWrapper(object item)
        {
            Item = item;
        }
        #region New Props
        public string Custnmbr { get; set; }

        [ObservableProperty]
        public string custname;


        [ObservableProperty]
        public string state;

        [ObservableProperty]
        public string zip;
        [ObservableProperty]
        public string custclas;

        [ObservableProperty]
        public string modelTitle;
        [ObservableProperty]
        public string areaErf;
        #endregion
        [ObservableProperty]
        bool isNew;
        [ObservableProperty]
        bool isUpdated;
        public ObservableCollection<ReadingWrapper> Readings { get; set; }
        public object Item { get; }
    }
}
