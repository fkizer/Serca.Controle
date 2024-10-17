using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Serca.Controle.Core.Application.ViewModels
{
    public class UserViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string? userm;
        private string? password;
        private ushort? ste;
        private ushort? depo;
        private string? erp;
        private Guid? codeMachine;

        public string? Userm
        {
            get => userm;
            set
            {
                if (userm != value)
                {
                    userm = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? Password
        {
            get => password;
            set
            {
                if (password != value)
                {
                    password = value;
                    OnPropertyChanged();
                }
            }
        }

        public ushort? Ste
        {
            get => ste;
            set
            {
                if (ste != value)
                {
                    ste = value;
                    OnPropertyChanged();
                }
            }
        }

        public ushort? Depo
        {
            get => depo;
            set
            {
                if (depo != value)
                {
                    depo = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? Erp
        {
            get => erp;
            set
            {
                if (erp != value)
                {
                    erp = value;
                    OnPropertyChanged();
                }
            }
        }

        public Guid? CodeMachine
        {
            get => codeMachine;
            set
            {
                if (codeMachine != value)
                {
                    codeMachine = value;
                    OnPropertyChanged();
                }
            }
        }

        public string? WasMigratedTo { get; set; }
        public bool MigrationPending { get; set; }

        public bool IsInitialized => !string.IsNullOrEmpty(Userm)
            && !string.IsNullOrEmpty(Password)
            && !string.IsNullOrEmpty(Erp);

        public bool IsAuthenticated => IsInitialized && Depo.HasValue && Ste.HasValue;

        private void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChangedEventHandler? handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
