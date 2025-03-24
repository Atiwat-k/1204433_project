using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MauiApp1.Model;
using Newtonsoft.Json;

public class StudentService
{
    private List<Course> _allCourses;

    public async Task LoadCoursesAsync()
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("courses.json");
        using var reader = new StreamReader(stream);
        var json = await reader.ReadToEndAsync();
        _allCourses = JsonConvert.DeserializeObject<List<Course>>(json);
    }

    public async Task<Student> GetStudentByIdAsync(string id)
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("students.json");
        using var reader = new StreamReader(stream);
        var json = await reader.ReadToEndAsync();
        var students = JsonConvert.DeserializeObject<List<Student>>(json);
        var student = students.FirstOrDefault(s => s.Id == id);

        if (student != null && _allCourses != null)
        {
            // ผูกข้อมูลหลักสูตร
            student.CurrentTerm.EnrolledCourses = _allCourses
                .Where(c => student.CurrentTerm.EnrolledCourseIds.Contains(c.CourseId))
                .ToList();

            foreach (var term in student.PreviousTerms)
            {
                term.EnrolledCourses = _allCourses
                    .Where(c => term.EnrolledCourseIds.Contains(c.CourseId))
                    .ToList();
            }
        }

        return student;
    }
}