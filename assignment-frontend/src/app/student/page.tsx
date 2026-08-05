"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import api from "@/lib/api";

interface Assignment {
  id: number; title: string; description: string; dueDate: string;
  maxMarks: number; subjectName: string; className: string;
}
interface Submission {
  id: number; assignmentId: number; answerText: string; submittedAt: string;
  marks: number | null; feedback: string | null; status: string;
}

export default function StudentDashboard() {
  const router = useRouter();
  const [assignments, setAssignments] = useState<Assignment[]>([]);
  const [submissions, setSubmissions] = useState<Submission[]>([]);
  const [answers, setAnswers] = useState<Record<number, string>>({});
  const [classId, setClassId] = useState<number | null>(null);

  useEffect(() => {
    if (localStorage.getItem("role") !== "Student") {
      router.push("/");
      return;
    }
    loadData();
  }, []);

  const loadData = async () => {
    const myClassId = Number(localStorage.getItem("classId"));
    if (!myClassId) return;
    setClassId(myClassId);
    const [a, s] = await Promise.all([
      api.get(`/assignments/class/${myClassId}`),
      api.get("/submissions/mine"),
    ]);
    setAssignments(a.data);
    setSubmissions(s.data);
  };

  const submit = async (assignmentId: number) => {
    const answerText = answers[assignmentId];
    if (!answerText) return;
    await api.post("/submissions", { assignmentId, answerText });
    loadData();
  };

  const getSubmissionFor = (assignmentId: number) =>
    submissions.find(s => s.assignmentId === assignmentId);

  const logout = () => {
    localStorage.clear();
    router.push("/");
  };

  return (
    <div className="min-h-screen bg-gray-100 p-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold">Student Dashboard</h1>
        <button onClick={logout} className="bg-red-500 text-white px-4 py-2 rounded">Logout</button>
      </div>

      <div className="bg-white p-6 rounded shadow">
        <h2 className="font-bold mb-4">My Assignments</h2>
        {assignments.length === 0 && <p className="text-sm text-gray-500">No published assignments yet.</p>}
        {assignments.map(a => {
          const sub = getSubmissionFor(a.id);
          const isPastDue = new Date() > new Date(a.dueDate);
          return (
            <div key={a.id} className="border-b py-4">
              <p className="font-semibold">{a.title} <span className="text-xs text-gray-500">({a.subjectName} — {a.className})</span></p>
              <p className="text-sm text-gray-700 mb-1">{a.description}</p>
              <p className="text-xs text-gray-500 mb-2">Due: {new Date(a.dueDate).toLocaleString()} | Max Marks: {a.maxMarks}</p>

              {sub ? (
                <div className="text-sm">
                  <p className="text-blue-700">Your answer: {sub.answerText}</p>
                  {sub.status === "Graded" ? (
                    <p className="text-green-700 mt-1">Marks: {sub.marks}/{a.maxMarks} — Feedback: {sub.feedback}</p>
                  ) : (
                    <p className="text-orange-600 mt-1">Status: Submitted (awaiting grading)</p>
                  )}
                  {!isPastDue && (
                    <div className="flex gap-2 mt-2">
                      <input placeholder="Update your answer" defaultValue={sub.answerText}
                        onChange={e => setAnswers({ ...answers, [a.id]: e.target.value })}
                        className="border p-1 rounded flex-1" />
                      <button onClick={() => submit(a.id)} className="bg-blue-600 text-white px-3 rounded text-sm">Update</button>
                    </div>
                  )}
                </div>
              ) : isPastDue ? (
                <p className="text-sm text-red-600">Deadline passed — no submission made.</p>
              ) : (
                <div className="flex gap-2">
                  <input placeholder="Type your answer" onChange={e => setAnswers({ ...answers, [a.id]: e.target.value })}
                    className="border p-1 rounded flex-1" />
                  <button onClick={() => submit(a.id)} className="bg-blue-600 text-white px-3 rounded text-sm">Submit</button>
                </div>
              )}
            </div>
          );
        })}
      </div>
    </div>
  );
}
