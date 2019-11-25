using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Dapper;

namespace WhoPingMVC {
    public class DatabaseSession {
        private static readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[0].ConnectionString;

        private static MySql.Data.MySqlClient.MySqlConnection GetConnection() {
            var conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString); conn.Open(); return conn;
        }

        public static async Task Init() {
            //Empty All database tables
            using (var connection = GetConnection()) {
                using (var transaction = connection.BeginTransaction()) {
                    try {
                        string[] tables = { "activity", "department", "scorelist", "staffrole" };
                        for (int i = 0; i < tables.Length; i++) {
                            await connection.ExecuteAsync($"TRUNCATE TABLE {tables[i]}");
                            System.Diagnostics.Debug.WriteLine($"TRUNCATE TABLE {tables[i]}");
                        }

                        //build staffrole
                        int rowsAffected = 0;
                        rowsAffected = await connection.ExecuteAsync("INSERT INTO StaffRole(Id,Role,Audit) VALUE('2000001', 2, '两个黄鹂鸣翠柳,牧童遥指杏花村')");
                        rowsAffected = await connection.ExecuteAsync("INSERT INTO StaffRole(Id,Role,Audit) VALUE('2000002', 2, '草木知春不久归')");
                        rowsAffected = await connection.ExecuteAsync("INSERT INTO StaffRole(Id,Role,Audit) VALUE('1000000',1,'')");
                        rowsAffected = await connection.ExecuteAsync("INSERT INTO StaffRole(Id,Role,Audit) VALUE('0000001',0,'离离原上草')");
                        rowsAffected = await connection.ExecuteAsync("INSERT INTO StaffRole(Id,Role,Audit) VALUE('0000002',0,'两个黄鹂鸣翠柳')");
                        rowsAffected = await connection.ExecuteAsync("INSERT INTO StaffRole(Id,Role,Audit) VALUE('0000003',0,'牧童遥指杏花村')");
                        rowsAffected = await connection.ExecuteAsync("INSERT INTO StaffRole(Id,Role,Audit) VALUE('0000004',0,'桃花潭水深千尺')");
                        rowsAffected = await connection.ExecuteAsync("INSERT INTO StaffRole(Id,Role,Audit) VALUE('0000005',0,'浅深红树见扬州')");
                        rowsAffected = await connection.ExecuteAsync("INSERT INTO StaffRole(Id,Role,Audit) VALUE('0000006',0,'草木知春不久归')");

                        //Department
                        rowsAffected = await connection.ExecuteAsync("INSERT INTO Department(Name,Block,DepartmentType) VALUE('离离原上草', '唐朝', 0)");
                        rowsAffected = await connection.ExecuteAsync("INSERT INTO Department(Name,Block,DepartmentType) VALUE('两个黄鹂鸣翠柳', '板块A', 1)");
                        rowsAffected = await connection.ExecuteAsync("INSERT INTO Department(Name,Block,DepartmentType) VALUE('牧童遥指杏花村', '板块A', 1)");
                        rowsAffected = await connection.ExecuteAsync("INSERT INTO Department(Name,Block,DepartmentType) VALUE('桃花潭水深千尺', '板块B', 1)");
                        rowsAffected = await connection.ExecuteAsync("INSERT INTO Department(Name,Block,DepartmentType) VALUE('浅深红树见扬州', '板块B', 1)");
                        rowsAffected = await connection.ExecuteAsync("INSERT INTO Department(Name,Block,DepartmentType) VALUE('草木知春不久归', '非板块', 1)");

                        //Assign Activity & ScoreList
                        var staffs = (await connection.QueryAsync<Models.Staff>("SELECT * FROM StaffRole")).ToList();
                        var departments = (await connection.QueryAsync<Models.Department>("SELECT * FROM Department")).ToDictionary(key => key.Name, value => value);
                        var departmentList = departments.Values.ToList();

                        staffs.ForEach(staff => {

                            int index = 0;
                            switch (staff.Role) {
                                case Models.Role.Branch:
                                    index = 0;
                                    departmentList.ForEach(async q => {
                                        await connection.ExecuteAsync("INSERT INTO Activity(Id,`Index`,SourceStaffId,TargetDepartment,Committed) VALUE(@Id,@Index,@SourceStaffId,@TargetDepartment,0)", new { Id = Guid.NewGuid().ToString(), Index = index++, SourceStaffId = staff.Id, TargetDepartment = q.Name });
                                    });
                                    break;
                                case Models.Role.Leader:
                                    string audit = staff.Audit; string[] auditDepartments;
                                    if (audit.Contains(",")) auditDepartments = audit.Split(','); else auditDepartments = new[] { audit };
                                    index = 0;

                                    departmentList.ForEach(async q => {
                                        if (auditDepartments.Contains(q.Name)) return;
                                        await connection.ExecuteAsync("INSERT INTO Activity(Id,`Index`,SourceStaffId,TargetDepartment,Committed) VALUE(@Id,@Index,@SourceStaffId,@TargetDepartment,0)", new { Id = Guid.NewGuid().ToString(), Index = index++, SourceStaffId = staff.Id, TargetDepartment = q.Name });
                                    });

                                    break;
                                case Models.Role.Department:
                                    index = 0;
                                    var department = departments[staff.Audit];
                                    switch (department.DepartmentType) {
                                        case Models.DepartmentType.Block:
                                            //Select All Support Departments
                                            var supportDepartments = departmentList.Where(q => q.DepartmentType == Models.DepartmentType.Support).ToList();
                                            supportDepartments.ForEach(async q => {
                                                await connection.ExecuteAsync("INSERT INTO Activity(Id,`Index`,SourceStaffId,TargetDepartment,BusinessBlock,Committed) VALUES(@Id,@Index,@SourceStaffId,@TargetDepartment,@BusinessBlock,0)", new { Id = Guid.NewGuid().ToString(), Index = index++, SourceStaffId = staff.Id, TargetDepartment = q.Name, BusinessBlock = q.Block });
                                            });
                                            var inBlockDepartments = departmentList.Where(q => q.Block == department.Block && q.DepartmentType == Models.DepartmentType.Block).ToList();
                                            inBlockDepartments.ForEach(async q => {
                                                await connection.ExecuteAsync("INSERT INTO Activity(Id,`Index`,SourceStaffId,TargetDepartment,BusinessBlock,Committed) VALUES(@Id,@Index,@SourceStaffId,@TargetDepartment,@BusinessBlock,0)", new { Id = Guid.NewGuid().ToString(), Index = index++, SourceStaffId = staff.Id, TargetDepartment = q.Name, BusinessBlock = q.Block });
                                            });
                                            var optionalDepartments = departmentList.Where(q => q.DepartmentType == Models.DepartmentType.Other).ToList();
                                            optionalDepartments.ForEach(async q => {
                                                await connection.ExecuteAsync("INSERT INTO Activity(Id,`Index`,SourceStaffId,TargetDepartment,BusinessBlock,Committed) VALUES(@Id,@Index,@SourceStaffId,@TargetDepartment,@BusinessBlock,0)", new { Id = Guid.NewGuid().ToString(), Index = index++, SourceStaffId = staff.Id, TargetDepartment = q.Name, BusinessBlock = q.Block });
                                            });
                                            break;
                                        default:
                                            departmentList.ForEach(async q => {
                                                await connection.ExecuteAsync("INSERT INTO Activity(Id,`Index`,SourceStaffId,TargetDepartment,BusinessBlock,Committed) VALUES(@Id,@Index,@SourceStaffId,@TargetDepartment,@BusinessBlock,0)", new { Id = Guid.NewGuid().ToString(), Index = index++, SourceStaffId = staff.Id, TargetDepartment = q.Name, BusinessBlock = q.Block });
                                            });

                                            break;
                                    }
                                    break;
                            }
                        });

                        transaction.Commit();
                    }catch (Exception ex) {
                        transaction.Rollback();
                        System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                    }
                }
            }
        }

        public static async Task<Models.Staff> GetStaff(string Id) {
            using (var connection = GetConnection()) {
                return await connection.QueryFirstOrDefaultAsync<Models.Staff>("SELECT * FROM Staff WHERE Id=@Id", Id);
            }
        }

        public static async Task<Models.Department> GetDepartment(string Name) {
            using (var connection = GetConnection()) {
                return await connection.QueryFirstOrDefaultAsync<Models.Department>("SELECT * FROM Department WHERE Name=@Name", Name);
            }
        }

        public static async Task<List<Models.ScoreList>> GetScoreList(string StaffId) {
            using (var connection = GetConnection()) {
                var x = await connection.QueryAsync<Models.ScoreList>("SELECT * FROM ScoreList WHERE StaffId=@StaffId", StaffId);
                return x.ToList();
            }
        }

        public static async Task<List<Models.Activity>> GetActivities(int[] Id) {
            using (var connection = GetConnection()) {
                var x = await connection.QueryAsync<Models.Activity>("SELECT * FROM Activity WHERE Id IN @Id", Id);
                return x.ToList();
            }
        }

        public static async Task<Models.Activity> GetActivity(int Id) {
            using (var connection = GetConnection()) {
                return await connection.QueryFirstOrDefaultAsync<Models.Activity>("SELECT * FROM Activity WHERE Id=@Id", Id);
            }
        }
    }
}
