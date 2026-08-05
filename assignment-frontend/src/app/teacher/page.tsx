"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import api from "@/lib/api";

interface MyAssignment {
  id: number; title: string; description: string; dueDate: string;
  maxMarks: number; status: string; classId: number; className: string;
  subjectId: number; subjectName: string;
}
interface MyTeacherAssignment {
  subjectId: number; subjectName: string; classId: number; className: string;
}
interface Submission {
  id: number; studentId: number; studentName: string; answerText: string;
  submittedAt: string; marks: number | null; feedback: string | null; status: string;
}

export default function TeacherDashboard() {
  const router = useRouter();
  const [assignments, setAssignments] = useState<MyAssignment[]>([]);
  const [myClasses, setMyClasses] = useState<MyTeacherAssignment[]>([]);
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [dueDate, setDueDate] = useState("");
  const [maxMarks, setMaxMarks] = useState(100);
  const [selectedPair, setSelectedPair] = useState("");

  const [viewingAssignmentId, setViewingAssignmentId] = useState<number | null>(null);
  const [submissions, setSubmissions] = useState<Submission[]>([]);
  const [gradeInputs, setGradeInputs] = useState<Record<number, { marks: string; feedback: string }>>({});

  useEffect(() => {
    if (localStorage.getItem("role") !== "Teacher") {
      router.push("/");
      return;
    }
    loadData();
  }, []);

  const loadData = async () => {
    const [a, t] = await Promise.all([
      api.get("/assignments/teacher"),
      api.get("/teacherassignments/mine"),
    ]);
    setAssignments(a.data);
    setMyClasses(t.data);
  };

  const createAssignment = async (e: React.FormEvent) => {
    e.preventDefault();
    const [subjectId, classId] = selectedPair.split("-").map(Number);
    await api.post("/assignments", { title, description, dueDate, maxMarks, classId, subjectId });
    setTitle(""); setDescription(""); setDueDate(""); setMaxMarks(100); setSelectedPair("");
    loadData();
  };

  const publish = async (id: number) => {
    await api.post(`/assignments/${id}/publish`);
    loadData();
  };

  const viewSubmissions = async (id: number) => {
    setViewingAssignmentId(id);
    const res = await api.get(`/submissions/assignment/${id}`);
    setSubmissions(res.data);
  };

  const grade = async (submissionId: number) => {
    const input = gradeInputs[submissionId];
    if (!input) return;
    await api.put(`/submissions/${submissionId}/grade`, {
      marks: Number(input.marks),
      feedback: input.feedback,
    });
    if (viewingAssignmentId) viewSubmissions(viewingAssignmentId);
  };

  const logout = () => {
    localStorage.clear();
    router.push("/");
  };

  return (
    <div className="min-h-screen bg-gray-100 p-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold">Teacher Dashboard</h1>
        <button onClick={logout} className="bg-red-500 text-white px-4 py-2 rounded">Logout</button>
      </div>

      <div className="bg-white p-6 rounded shadow mb-6">
        <h2 className="font-bold mb-4">Create Assignment</h2>
        <form onSubmit={createAssignment} className="grid grid-cols-1 md:grid-cols-2 gap-3">
          <input placeholder="Title" value={title} onChange={e => setTitle(e.target.value)} className="border p-2 rounded" required />
          <select value={selectedPair} onChange={e => setSelectedPair(e.target.value)} className="border p-2 rounded" required>
            <option value="">Select Subject + Class</option>
            {myClasses.map((mc, i) => (
              <option key={i} value={`${mc.subjectId}-${mc.classId}`}>{mc.subjectName} — {mc.className}</option>
            ))}
          </select>
          <textarea placeholder="Description" value={description} onChange={e => setDescription(e.target.value)} className="border p-2 rounded md:col-span-2" required />
          <input type="datetime-local" value={dueDate} onChange={e => setDueDate(e.target.value)} className="border p-2 rounded" required />
          <input type="number" placeholder="Max Marks" value={maxMarks} onChange={e => setMaxMarks(Number(e.target.value))} className="border p-2 rounded" required />
          <button className="bg-blue-600 text-white py-2 rounded md:col-span-2">Create (as Draft)</button>
        </form>
        {myClasses.length === 0 && <p className="text-sm text-orange-600 mt-2">You have no Subject+Class assignments yet. Ask Admin to assign you.</p>}
      </div>

      <div className="bg-white p-6 rounded shadow">
        <h2 className="font-bold mb-4">My Assignments</h2>
        <table className="w-full text-sm">
          <thead>
            <tr className="text-left border-b">
              <th className="pb-2">Title</th><th>Class</th><th>Subject</th><th>Status</th><th>Due</th><th></th>
            </tr>
          </thead>
          <tbody>
            {assignments.map(a => (
              <tr key={a.id} className="border-b">
                <td className="py-2">{a.title}</td>
                <td>{a.className}</td>
                <td>{a.subjectName}</td>
                <td>{a.status}</td>
                <td>{new Date(a.dueDate).toLocaleString()}</td>
                <td className="space-x-2">
                  {a.status === "Draft" && <button onClick={() => publish(a.id)} className="bg-green-600 text-white px-3 py-1 rounded text-xs">Publish</button>}
                  <button onClick={() => viewSubmissions(a.id)} className="bg-gray-600 text-white px-3 py-1 rounded text-xs">Submissions</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {viewingAssignmentId && (
        <div className="bg-white p-6 rounded shadow mt-6">
          <h2 className="font-bold mb-4">Submissions</h2>
          {submissions.length === 0 && <p className="text-sm text-gray-500">No submissions yet.</p>}
          {submissions.map(s => (
            <div key={s.id} className="border-b py-3">
              <p className="font-semibold">{s.studentName}</p>
              <p className="text-sm text-gray-700 mb-2">{s.answerText}</p>
              {s.status === "Graded" ? (
                <p className="text-sm text-green-700">Graded: {s.marks} marks — {s.feedback}</p>
              ) : (
                <div className="flex gap-2">
                  <input placeholder="Marks" type="number" className="border p-1 rounded w-24"
                    onChange={e => setGradeInputs({ ...gradeInputs, [s.id]: { ...gradeInputs[s.id], marks: e.target.value } })} />
                  <input placeholder="Feedback" className="border p-1 rounded flex-1"
                    onChange={e => setGradeInputs({ ...gradeInputs, [s.id]: { ...gradeInputs[s.id], feedback: e.target.value } })} />
                  <button onClick={() => grade(s.id)} className="bg-blue-600 text-white px-3 rounded text-sm">Save</button>
                </div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}