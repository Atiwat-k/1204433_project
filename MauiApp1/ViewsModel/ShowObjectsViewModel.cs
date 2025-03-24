using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.Model;
using MauiApp1.Services;
using System.Linq;

namespace MauiApp1.ViewModel
{
    public partial class ShowObjectsViewModel : ObservableObject
    {
        private readonly StudentService _studentService;

        [ObservableProperty]
        private ObservableCollection<Term> availableTerms = new();

        [ObservableProperty]
        private Term selectedTerm;

        [ObservableProperty]
        private ObservableCollection<CourseGroup> groupedCourses = new();

        [ObservableProperty]
        private string userId;

        [ObservableProperty]
        private Profile studentProfile;

        [ObservableProperty]
        private bool isLoading;

        public ShowObjectsViewModel(StudentService studentService)
        {
            _studentService = studentService;
        }

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;
                
                // โหลดข้อมูลหลักสูตรก่อน
                await _studentService.LoadCoursesAsync();
                
                var student = await _studentService.GetStudentByIdAsync(UserId);
                if (student != null)
                {
                    StudentProfile = student.Profile;

                    // รวบรวมและแสดงข้อมูลเทอมทั้งหมด
                    UpdateAvailableTerms(student);

                    // จัดกลุ่มข้อมูลหลักสูตร
                    GroupCourses(student);

                    // เลือกเทอมปัจจุบันเป็นค่าเริ่มต้น
                    if (AvailableTerms.Count > 0)
                    {
                        SelectedTerm = AvailableTerms.First();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading data: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateAvailableTerms(Student student)
        {
            AvailableTerms.Clear();
            
            if (student.CurrentTerm != null)
            {
                AvailableTerms.Add(student.CurrentTerm);
            }
            
            if (student.PreviousTerms != null)
            {
                foreach (var term in student.PreviousTerms)
                {
                    AvailableTerms.Add(term);
                }
            }
        }

        private void GroupCourses(Student student)
        {
            var allTerms = new List<Term>();
            
            if (student.CurrentTerm != null)
            {
                allTerms.Add(student.CurrentTerm);
            }
            
            if (student.PreviousTerms != null)
            {
                allTerms.AddRange(student.PreviousTerms);
            }

            // สร้างกลุ่มวิชาจากหลักสูตรทั้งหมดที่นักศึกษาเคยลงทะเบียน
            var courseGroups = allTerms
                .SelectMany(t => t.EnrolledCourses)
                .GroupBy(c => c.CourseId)
                .Select(g => new CourseGroup
                {
                    CourseId = g.Key,
                    CourseName = g.First().CourseName,
                    Credits = (int)g.First().Credits,
                    TermsOffered = allTerms
                        .Where(t => t.EnrolledCourses.Any(ec => ec.CourseId == g.Key))
                        .ToList()
                })
                .OrderBy(cg => cg.CourseId)
                .ToList();

            GroupedCourses = new ObservableCollection<CourseGroup>(courseGroups);
        }

        partial void OnSelectedTermChanged(Term value)
        {
            if (value != null)
            {
                // สามารถเพิ่มการกรองข้อมูลตามเทอมที่เลือกได้ที่นี่
                // หรือใช้ SelectedTerm ใน View โดยตรง
            }
        }

        [RelayCommand]
        private void SelectTerm(Term term)
        {
            SelectedTerm = term;
        }

        [RelayCommand]
        private async Task RefreshData()
        {
            await LoadDataAsync();
        }
    }
}