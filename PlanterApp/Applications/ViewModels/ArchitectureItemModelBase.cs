using PlanterApp.Applications.Services;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Waf.Applications;
using System.Windows.Input;

namespace PlanterApp.Applications.ViewModels
{
    internal abstract class ArchitectureItemModelBase<TView> : ViewModel<TView> where TView : IView
    {
        protected ICommandService _commandService;
        //public ICommandService CommandService
        //{
        //    get { return _commandService; }
        //    set { SetProperty(ref _commandService, value); }
        //}

        protected ICommand _changeTypeCommand;
        //public ICommand ChangeTypeCommand
        //{
        //    get { return _changeTypeCommand; }
        //    set { SetProperty(ref _changeTypeCommand, value); }
        //}

        private IArchitectureItemModel _parent;
        public IArchitectureItemModel Parent
        {
            get { return _parent; }
            set { SetProperty(ref _parent, value); }
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                SetProperty(ref _isExpanded, value);

                if (_isExpanded && _parent != null)
                {
                    _parent.IsExpanded = true;
                }
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        private bool _focusable;
        public bool Focusable
        {
            get { return _focusable; }
            set { SetProperty(ref _focusable, value); }
        }

        private ObservableCollection<object> _children = new ObservableCollection<object>();
        public ObservableCollection<object> Children
        {
            get { return _children; }
        }

        private ObservableCollection<PlanterMenuItem> _changeTypeMenu;
        public ObservableCollection<PlanterMenuItem> ChangeTypeMenu
        {
            get { return _changeTypeMenu; }
            set { SetProperty(ref _changeTypeMenu, value); }
        }

        public ICollection PlanterMenuItems
        {
            get
            {
                return GetPlanterMenuItems();
            }
        }

        public event EventHandler StatesUpdated;

        protected ArchitectureItemModelBase(TView view, ICommandService commandService)
            : base(view)
        {
            _commandService = commandService;
        }

        protected void RaiseStatesUpdated()
        {
            var handler = StatesUpdated;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected PlanterMenuItem CreateChangeTypeMenuItem(object targetState, Key key)
        {
            return new PlanterMenuItem(_changeTypeCommand, GetCommandParam(targetState), key, ModifierKeys.Alt);
        }

        protected PlanterMenuCommandParam GetCommandParam(object targetState)
        {
            return new PlanterMenuCommandParam
                {
                    TargetState = targetState,
                    Source = this
                };
        }

        //protected PlanterMenuItem CreateMenuItem(ICommand command, object targetState, Key key, object source)
        //{
        //    var commandParameter = new ArchitectureMenuCommandParam
        //        {
        //            TargetState = targetState,
        //            Source = source
        //        };

        //    return new PlanterMenuItem(command, commandParameter, key, ModifierKeys.Alt);
        //}

        protected abstract ICollection GetPlanterMenuItems();
    }
}
