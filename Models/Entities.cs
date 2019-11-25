using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhoPingMVC.Models {
    public class Entities {
    }

    public enum Role {
        Department = 0,
        Branch = 1,
        Leader = 2
    }

    public class Staff {
        public string Id { get; set; }
        public Role Role { get; set; }
        public string Audit { get; set; }
    }

    public enum DepartmentType {
        Support = 0,
        Block = 1,
        Other = 2
    }

    public class Department {
        public string Name { get; set; }
        public string Block { get; set; }
        public DepartmentType DepartmentType { get; set; }
    }

    public class ScoreList {
        public string StaffId { get; set; }
        public int ActivityId { get; set; }
        public bool Optional { get; set; } = false;
    }

    public class Activity {
        public string Id { get; set; }
        public int Index { get; set; } //似乎没有用
        public string SourceStaffId { get; set; }
        public string TargetDepartment { get; set; }
        public int Profession { get; set; }
        public int Duty { get; set; }
        public int Cooperation { get; set; }
        public int Result { get; set; }
        public int Score { get; set; }
        public string ScoreText { get; set; }
        public DateTime CommittedDateTime { get; set; }
        public bool Committed { get; set; }
        public bool Optional { get; set; } = false;
    }
}
