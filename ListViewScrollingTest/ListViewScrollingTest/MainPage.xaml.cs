using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ListViewScrollingTest
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new ProjectsViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var jsonString = "[{\"ProjectName\":\"Test sag\",\"ProjectReference\":\"10072\",\"CustomerName\":\"Test firma\",\"FullAddress\":\"Testvej 3\",\"StartDate\":\"2017-02-02T00:00:00\",\"StartTime\":\"\"},{\"ProjectName\":\"aaa\",\"ProjectReference\":\"10077\",\"CustomerName\":\"Test firma\",\"FullAddress\":\"Testvej 12\",\"StartDate\":\"2017-02-08T00:00:00\",\"StartTime\":\"\"},{\"ProjectName\":\"Test\",\"ProjectReference\":\"10082\",\"CustomerName\":\"Test firma\",\"FullAddress\":\"Testvej 50\",\"StartDate\":\"2017-02-16T00:00:00\",\"StartTime\":\"\"},{\"ProjectName\":\"Test\",\"ProjectReference\":\"10085\",\"CustomerName\":\"Testvej boligselskab\",\"FullAddress\":\"Testvej 14\",\"StartDate\":\"2017-02-24T00:00:00\",\"StartTime\":\"\"},{\"ProjectName\":\"Test\",\"ProjectReference\":\"10086\",\"CustomerName\":\"Testing\",\"FullAddress\":\"Testevej 14\",\"StartDate\":\"2017-02-27T00:00:00\",\"StartTime\":\"\"},{\"ProjectName\":\"Test1\",\"ProjectReference\":\"10087\",\"CustomerName\":\"Plejecenter testlyst\",\"FullAddress\":\"Testlystvej 11\",\"StartDate\":\"2017-02-27T00:00:00\",\"StartTime\":\"\"},{\"ProjectName\":\"Test2\",\"ProjectReference\":\"10088\",\"CustomerName\":\"Charlie\",\"FullAddress\":\"Testvej 50\",\"StartDate\":\"2017-02-27T00:00:00\",\"StartTime\":\"\"},{\"ProjectName\":\"Test\",\"ProjectReference\":\"10089\",\"CustomerName\":\"Standard Debitor\",\"FullAddress\":\"[Mangler]\",\"StartDate\":\"2017-03-16T00:00:00\",\"StartTime\":\"\"},{\"ProjectName\":\"Test\",\"ProjectReference\":\"10090\",\"CustomerName\":\"Standard Debitor\",\"FullAddress\":\"[Mangler]\",\"StartDate\":\"2017-03-16T00:00:00\",\"StartTime\":\"\"},{\"ProjectName\":\"Test\",\"ProjectReference\":\"10091\",\"CustomerName\":\"Standard Debitor\",\"FullAddress\":\"[Mangler]\",\"StartDate\":\"2017-03-16T00:00:00\",\"StartTime\":\"\"},{\"ProjectName\":\"Test\",\"ProjectReference\":\"10092\",\"CustomerName\":\"Tester\",\"FullAddress\":\"Testvej 11\",\"StartDate\":\"2017-03-16T00:00:00\",\"StartTime\":\"\"},{\"ProjectName\":\"Test\",\"ProjectReference\":\"10093\",\"CustomerName\":\"Plejehjemmet test\",\"FullAddress\":\"Testvej 90\",\"StartDate\":\"2017-03-16T00:00:00\",\"StartTime\":\"\"},{\"ProjectName\":\"Test\",\"ProjectReference\":\"10094\",\"CustomerName\":\"Plejehjemmet test\",\"FullAddress\":\"Testvej 90\",\"StartDate\":\"2017-03-16T00:00:00\",\"StartTime\":\"\"}]";
            var projects = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ProjectDto>>(jsonString);
            var viewModel = BindingContext as ProjectsViewModel;
            if (viewModel != null)
                viewModel.OriginalProjects = projects;

            Task.Delay(5000).ContinueWith((x) =>
            {
                //  Device.BeginInvokeOnMainThread(Acr.UserDialogs.UserDialogs.Instance.HideLoading);
                Search();
            });
        }

        private void Search(string inputVal = null)
        {
            var viewModel = BindingContext as ProjectsViewModel;

            if (viewModel != null)
            {
                var projects = viewModel.OriginalProjects.Where(p => !string.IsNullOrEmpty(inputVal) ? p.ProjectName.Contains(inputVal) : true);

                var orderedProjects = projects.OrderBy(p => p.StartDate);

                Device.BeginInvokeOnMainThread(() =>
                {
                    foreach (ProjectDto project in orderedProjects)
                    {
                        var coll = viewModel.Projects.FirstOrDefault(c => c.Key == project.StartDate);

                        if (coll == null)
                            viewModel.Projects.Add(coll = new ObservableCollectionWithDateKey { Key = project.StartDate });

                        coll.Add(project);
                    }

                    var group = viewModel.Projects.Last();
                    if (group != null)
                        ProjectsListView.ScrollTo(group.First(), group, ScrollToPosition.Start, false);
                });
            }
        }

        private void ProjectsListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
        }
    }

    internal class ProjectsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ObservableCollectionWithDateKey> _projects;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<ProjectDto> OriginalProjects { get; set; }

        public ObservableCollection<ObservableCollectionWithDateKey> Projects
        {
            get { return _projects; }
            set
            {
                _projects = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Projects)));
            }
        }

        public ProjectsViewModel()
        {
            Projects = new ObservableCollection<ObservableCollectionWithDateKey>();
        }
    }

    public class ProjectDto : INotifyPropertyChanged
    {
        public string ProjectName { get; set; }
        public string ProjectReference { get; set; }
        public string CustomerName { get; set; }
        public string FullAddress { get; set; }
        public DateTime StartDate { get; set; }
        public string StartTime { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    internal class ObservableCollectionWithDateKey : ObservableCollection<ProjectDto>
    {
        public DateTime Key { get; set; }
    }
}