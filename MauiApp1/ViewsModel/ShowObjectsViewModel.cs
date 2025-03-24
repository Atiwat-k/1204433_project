using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApp1.Model;
using MauiApp1.Services;

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
        private ObservableCollection<EnrolledCourse> displayedCourses = new();

        [ObservableProperty]
        private string userId;

        // เพิ่ม property สำหรับข้อมูลโปรไฟล์นักศึกษา
        [ObservableProperty]
        private Profile studentProfile;

        // เพิ่ม property สำหรับเก็บเทอมปัจจุบันและเทอมก่อนหน้า
        [ObservableProperty]
        private Term currentTerm;

        [ObservableProperty]
        private Term previousTerm1;

        [ObservableProperty]
        private Term previousTerm2;

        public ShowObjectsViewModel(StudentService studentService)
        {
            _studentService = studentService;
        }

        public async Task LoadDataAsync()
        {
            var student = await _studentService.GetStudentByIdAsync(UserId);
            if (student != null)
            {
                // เก็บข้อมูลโปรไฟล์นักศึกษา
                StudentProfile = student.Profile;

                // เตรียมข้อมูลเทอม
                CurrentTerm = student.CurrentTerm;
                if (student.PreviousTerms.Count > 0)
                    PreviousTerm1 = student.PreviousTerms[0];
                if (student.PreviousTerms.Count > 1)
                    PreviousTerm2 = student.PreviousTerms[1];

                // เซ็ตค่าเริ่มต้นเป็นเทอมปัจจุบัน
                SelectedTerm = CurrentTerm;
                UpdateDisplayedCourses();

                Debug.WriteLine($"Student loaded: {student.Profile.Name}");
            }
            else
            {
                Debug.WriteLine("Student not found.");
            }
        }

        // เมธอดอัปเดตรายวิชาตามเทอมที่เลือก
        partial void OnSelectedTermChanged(Term value)
        {
            UpdateDisplayedCourses();
        }

        private void UpdateDisplayedCourses()
        {
            if (SelectedTerm != null)
            {
                DisplayedCourses = new ObservableCollection<EnrolledCourse>(SelectedTerm.EnrolledCourses);
            }
        }

        // Command สำหรับเลือกเทอมปัจจุบัน
        [RelayCommand]
        private void SelectCurrentTerm()
        {
            SelectedTerm = CurrentTerm;
        }

        // Command สำหรับเลือกเทอมก่อนหน้า 1
        [RelayCommand]
        private void SelectPreviousTerm1()
        {
            SelectedTerm = PreviousTerm1;
        }

        // Command สำหรับเลือกเทอมก่อนหน้า 2
        [RelayCommand]
        private void SelectPreviousTerm2()
        {
            SelectedTerm = PreviousTerm2;
        }
    }
}