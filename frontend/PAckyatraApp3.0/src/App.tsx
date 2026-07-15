import React, { useState } from 'react';
import { AuthProvider, useAuth } from './context/AuthContext';
import LoginScreen from './screens/LoginScreen';
import LandingScreen from './screens/LandingScreen';
import JobDetailScreen from './screens/JobDetailScreen';
import { Job } from './types';

const MainAppContent: React.FC = () => {
  const { user, loading, logout } = useAuth();
  const [selectedJob, setSelectedJob] = useState<Job | null>(null);

  const [jobs, setJobs] = useState<Job[]>(() => {
    const saved = localStorage.getItem('packyatra_jobs');
    if (saved) {
      try {
        return JSON.parse(saved);
      } catch (e) {
        // default fallback
      }
    }
    return [
      { id: 1, name: 'Badri Narayanan', location: 'Bangalore, Karnataka, India', type: 'Packers And Movers', date: 'Tue 17 Feb', time: '02:00 PM', status: 'pending' },
      { id: 2, name: 'Rajesh Kumar', location: 'Mumbai, Maharashtra, India', type: 'Household Moving', date: 'Wed 18 Feb', time: '10:30 AM', status: 'transit' },
      { id: 3, name: 'Anita Sharma', location: 'Delhi, NCR, India', type: 'Office Relocation', date: 'Thu 19 Feb', time: '09:00 AM', status: 'pending' },
    ];
  });

  const updateJobStatus = (id: number, status: string) => {
    const updated = jobs.map(j => j.id === id ? { ...j, status } : j);
    setJobs(updated);
    localStorage.setItem('packyatra_jobs', JSON.stringify(updated));
    if (selectedJob && selectedJob.id === id) {
      setSelectedJob({ ...selectedJob, status });
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen w-full flex flex-col items-center justify-center bg-slate-50">
        <div className="flex flex-col items-center gap-3">
          <svg className="animate-spin h-10 w-10 text-blue-600" fill="none" viewBox="0 0 24 24">
            <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
            <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" />
          </svg>
          <span className="text-xs font-extrabold uppercase tracking-widest text-slate-400">PackYatra Portal</span>
        </div>
      </div>
    );
  }

  // Routing Switch
  if (!user) {
    return <LoginScreen onLoginSuccess={() => setSelectedJob(null)} />;
  }

  if (selectedJob) {
    // Find the latest status of this job from the list
    const currentJob = jobs.find(j => j.id === selectedJob.id) || selectedJob;
    return (
      <JobDetailScreen
        job={currentJob}
        onBack={() => setSelectedJob(null)}
        onUpdateStatus={(status) => updateJobStatus(currentJob.id, status)}
        onLogout={() => {
          logout();
          setSelectedJob(null);
        }}
      />
    );
  }

  return (
    <LandingScreen
      jobs={jobs}
      onSelectJob={(job) => setSelectedJob(job)}
      onLogout={() => setSelectedJob(null)}
    />
  );
};

export default function App() {
  return (
    <AuthProvider>
      <MainAppContent />
    </AuthProvider>
  );
}
