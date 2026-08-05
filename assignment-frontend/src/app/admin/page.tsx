"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import api from "@/lib/api";

interface ClassItem {
  id: number;
  name: string;
  section: string;
}

interface SubjectItem {
  id: number;
  name: string;
}

interface UserItem {
  id: number;
  fullName: string;
  email: string;
  role: string;
}

interface TeacherAssignmentItem {
  id: number;
  teacherId: number;
  teacherName: string;
  subjectId: number;
  subjectName: string;
  classId: number;
  className: string;
}

export default function AdminDashboard() {
  const router = useRouter();
  const [classes, setClasses] = useState<ClassItem[]>([]);
  const [subjects, setSubjects] = useState<SubjectItem[]>([]);
  const [users, setUsers] = useState<UserItem[]>([]);
  const [teacherAssignments, setTeacherAssignments] = useState<TeacherAssignmentItem[]>([]);
  const [selectedTeacherId, setSelectedTeacherId] = useState("");
  const [selectedSubjectId, setSelectedSubjectId] = useState("");
  const [selectedClassId, setSelectedClassId] = useState("");
  const [className, setClassName] = useState("");
  const [classSection, setClassSection] = useState("");
  const [subjectName, setSubjectName] = useState("");

  useEffect(() => {
    if (localStorage.getItem("role") !== "Admin") {
      router.push("/");
      return;
    }
    loadData();
  }, []);

  const loadData = async () => {
    const [c, s, u, t] = await Promise.all([
      api.get("/classes"),
      api.get("/subjects"),
      api.get("/users"),
      api.get("/teacherassignments"),
    ]);
    setClasses(c.data);
    setSubjects(s.data);
    setUsers(u.data);
    setTeacherAssignments(t.data);
  };

  const addClass = async (e: React.FormEvent) => {
    e.preventDefault();
    await api.post("/classes", { name: className, section: classSection });
    setClassName("");
    setClassSection("");
    loadData();
  };

  const addSubject = async (e: React.FormEvent) => {
    e.preventDefault();
    await api.post("/subjects", { name: subjectName });
    setSubjectName("");
    loadData();
  };

  const assignTeacher = async (e: React.FormEvent) => {
    e.preventDefault();
    await api.post("/teacherassignments", {
      teacherId: Number(selectedTeacherId),
      subjectId: Number(selectedSubjectId),
      classId: Number(selectedClassId),
    });
    setSelectedTeacherId(""); setSelectedSubjectId(""); setSelectedClassId("");
    loadData();
  };

  const logout = () => {
    localStorage.clear();
    router.push("/");
  };

  return (
    <div className="min-h-screen bg-gray-100 p-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold">Admin Dashboard</h1>
        <button onClick={logout} className="bg-red-500 text-white px-4 py-2 rounded">Logout</button>
      </div>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div className="bg-white p-6 rounded shadow">
          <h2 className="font-bold mb-4">Classes</h2>
          <form onSubmit={addClass} className="flex gap-2 mb-4">
            <input placeholder="Name (e.g. Grade 10)" value={className} onChange={e => setClassName(e.target.value)} className="border p-2 rounded flex-1" required />
            <input placeholder="Section (e.g. A)" value={classSection} onChange={e => setClassSection(e.target.value)} className="border p-2 rounded w-20" required />
            <button className="bg-blue-600 text-white px-4 rounded">Add</button>
          </form>
          <ul className="space-y-1">
            {classes.map(c => <li key={c.id} className="text-sm">{c.name} {c.section}</li>)}
          </ul>
        </div>
        <div className="bg-white p-6 rounded shadow">
          <h2 className="font-bold mb-4">Subjects</h2>
          <form onSubmit={addSubject} className="flex gap-2 mb-4">
            <input placeholder="Name (e.g. Mathematics)" value={subjectName} onChange={e => setSubjectName(e.target.value)} className="border p-2 rounded flex-1" required />
            <button className="bg-blue-600 text-white px-4 rounded">Add</button>
          </form>
          <ul className="space-y-1">
            {subjects.map(s => <li key={s.id} className="text-sm">{s.name}</li>)}
          </ul>
        </div>
        <div className="bg-white p-6 rounded shadow md:col-span-2">
          <h2 className="font-bold mb-4">Users</h2>
          <table className="w-full text-sm">
            <thead>
              <tr className="text-left border-b">
                <th className="pb-2">Name</th>
                <th className="pb-2">Email</th>
                <th className="pb-2">Role</th>
              </tr>
            </thead>
            <tbody>
              {users.map(u => (
                <tr key={u.id} className="border-b">
                  <td className="py-2">{u.fullName}</td>
                  <td className="py-2">{u.email}</td>
                  <td className="py-2">{u.role}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        <div className="bg-white p-6 rounded shadow md:col-span-2">
          <h2 className="font-bold mb-4">Assign Teacher to Subject + Class</h2>
          <form onSubmit={assignTeacher} className="flex gap-2 mb-4">
            <select value={selectedTeacherId} onChange={e => setSelectedTeacherId(e.target.value)} className="border p-2 rounded flex-1" required>
              <option value="">Select Teacher</option>
              {users.filter(u => u.role === "Teacher").map(t => <option key={t.id} value={t.id}>{t.fullName}</option>)}
            </select>
            <select value={selectedSubjectId} onChange={e => setSelectedSubjectId(e.target.value)} className="border p-2 rounded flex-1" required>
              <option value="">Select Subject</option>
              {subjects.map(s => <option key={s.id} value={s.id}>{s.name}</option>)}
            </select>
            <select value={selectedClassId} onChange={e => setSelectedClassId(e.target.value)} className="border p-2 rounded flex-1" required>
              <option value="">Select Class</option>
              {classes.map(c => <option key={c.id} value={c.id}>{c.name} {c.section}</option>)}
            </select>
            <button className="bg-blue-600 text-white px-4 rounded">Assign</button>
          </form>
          <table className="w-full text-sm">
            <thead>
              <tr className="text-left border-b">
                <th className="pb-2">Teacher</th>
                <th className="pb-2">Subject</th>
                <th className="pb-2">Class</th>
              </tr>
            </thead>
            <tbody>
              {teacherAssignments.map(ta => (
                <tr key={ta.id} className="border-b">
                  <td className="py-2">{ta.teacherName}</td>
                  <td className="py-2">{ta.subjectName}</td>
                  <td className="py-2">{ta.className}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
