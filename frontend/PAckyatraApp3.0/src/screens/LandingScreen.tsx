import React, { useState } from 'react';
import { useAuth } from '../context/AuthContext';
import { Job } from '../types';
import { 
  Search, 
  MapPin, 
  Calendar, 
  Clock, 
  ArrowRight, 
  LogOut, 
  Home, 
  Briefcase, 
  Wallet, 
  Star, 
  Shield, 
  Heart, 
  Truck, 
  TrendingUp, 
  CheckCircle,
  FileText,
  User,
  Map
} from 'lucide-react';

interface LandingScreenProps {
  jobs: Job[];
  onSelectJob: (job: Job) => void;
  onLogout: () => void;
}

const LandingScreen: React.FC<LandingScreenProps> = ({ jobs, onSelectJob, onLogout }) => {
  const [activeTab, setActiveTab] = useState('All Jobs');
  const [activeChip, setActiveChip] = useState('PENDING');
  const [searchQuery, setSearchQuery] = useState('');
  const [showLogoutModal, setShowLogoutModal] = useState(false);
  const { logout, user } = useAuth();

  const tabs = ['All Jobs', 'Today', 'Tomorrow'];
  const chips = [
    { label: 'PENDING', icon: Clock, value: 'PENDING' },
    { label: 'IN-TRANSIT', icon: Truck, value: 'IN-TRANSIT' },
    { label: 'COMPLETED', icon: CheckCircle, value: 'COMPLETED' },
  ];

  // Filtering logic
  const filteredJobs = jobs.filter(job => {
    // 1. Search Query
    const query = searchQuery.toLowerCase().trim();
    const matchesSearch = job.name.toLowerCase().includes(query) || job.location.toLowerCase().includes(query);
    
    // 2. Tab Filter
    let matchesTab = true;
    if (activeTab === 'Today') {
      matchesTab = job.date.includes('17 Feb'); // Assuming 'Tue 17 Feb' is today in this snapshot
    } else if (activeTab === 'Tomorrow') {
      matchesTab = job.date.includes('18 Feb');
    }

    // 3. Chip Filter
    let matchesChip = true;
    if (activeChip === 'PENDING') {
      matchesChip = job.status === 'pending';
    } else if (activeChip === 'IN-TRANSIT') {
      matchesChip = job.status === 'transit';
    } else if (activeChip === 'COMPLETED') {
      matchesChip = job.status === 'completed' || job.status === 'delivered';
    }

    return matchesSearch && matchesTab && matchesChip;
  });

  const handleLogoutConfirm = () => {
    logout();
    onLogout();
  };

  return (
    <div className="min-h-screen w-full bg-white text-slate-800 flex flex-col pb-24 md:pb-8 select-none font-sans">
      
      {/* Upper Brand Cover Banner - Clean White Design */}
      <div className="w-full bg-white pt-6 pb-24 px-4 sm:px-6 relative overflow-hidden border-b border-slate-100">
        {/* Modern grid & glow pattern overlays */}
        <div className="absolute inset-0 bg-[linear-gradient(to_right,#f1f5f9_1px,transparent_1px),linear-gradient(to_bottom,#f1f5f9_1px,transparent_1px)] bg-[size:3rem_3rem] opacity-70" />
        <div className="absolute top-[-50px] right-[-50px] w-72 h-72 rounded-full bg-blue-50/50 blur-3xl" />
        
        <div className="max-w-4xl w-full mx-auto relative z-10">
          {/* Header Row */}
          <header className="flex justify-between items-center mb-6">
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 rounded-xl bg-gradient-to-tr from-blue-600 to-indigo-600 flex items-center justify-center text-white shadow-lg shadow-blue-500/10">
                <Truck className="w-5 h-5" />
              </div>
              <div className="flex flex-col">
                <span className="text-[10px] uppercase tracking-widest text-slate-500 font-extrabold leading-tight">
                  Supervisor Panel
                </span>
                <h1 className="text-lg font-black text-slate-900 tracking-tight leading-tight">
                  Packyatra
                </h1>
              </div>
            </div>

            <div className="flex items-center gap-3">
              <div className="flex items-center gap-1.5 bg-slate-100 border border-slate-200 px-3.5 py-1.5 rounded-full shadow-sm text-xs font-bold text-slate-700">
                <Star className="w-3.5 h-3.5 text-amber-500 fill-amber-500" />
                <span>4.9 ★</span>
              </div>
            </div>
          </header>

          {/* Welcome Greeting Widget */}
          <div className="flex items-center justify-between mt-2">
            <div>
              <p className="text-slate-500 text-xs font-medium">Welcome back,</p>
              <h2 className="text-xl font-bold text-slate-900 tracking-tight mt-0.5">
                {user?.firstName || 'Raja Sahni'}
              </h2>
            </div>
            <div className="text-right">
              <span className="text-[11px] font-bold text-blue-600 block tracking-widest uppercase">
                ID: PY-81729
              </span>
              <span className="text-slate-500 text-xs mt-0.5 block font-medium">
                Active Zone: South Region
              </span>
            </div>
          </div>
        </div>
      </div>

      {/* Main Content Area overlapping the banner */}
      <div className="flex-1 max-w-4xl w-full mx-auto px-4 sm:px-6 relative -mt-16 z-20">
        
        {/* KPI Dashboard Summary Cards Widget */}
        <div className="grid grid-cols-3 gap-3 mb-6">
          <div className="bg-white rounded-2xl border border-slate-200/60 p-3.5 shadow-sm hover:shadow-md transition-shadow">
            <div className="flex items-center justify-between mb-1">
              <span className="text-slate-400 text-[10px] font-bold uppercase tracking-wider">Pending</span>
              <div className="p-1 rounded-lg bg-amber-50 text-amber-600">
                <Clock className="w-4 h-4" />
              </div>
            </div>
            <span className="text-xl font-black text-slate-900">
              {jobs.filter(j => j.status === 'pending').length < 10 
                ? `0${jobs.filter(j => j.status === 'pending').length}` 
                : jobs.filter(j => j.status === 'pending').length}
            </span>
            <span className="text-[9px] font-bold text-slate-500 block mt-0.5">Awaiting dispatch</span>
          </div>

          <div className="bg-white rounded-2xl border border-slate-200/60 p-3.5 shadow-sm hover:shadow-md transition-shadow">
            <div className="flex items-center justify-between mb-1">
              <span className="text-slate-400 text-[10px] font-bold uppercase tracking-wider">Transit</span>
              <div className="p-1 rounded-lg bg-blue-50 text-blue-600">
                <Truck className="w-4 h-4" />
              </div>
            </div>
            <span className="text-xl font-black text-slate-900">
              {jobs.filter(j => j.status === 'transit').length < 10 
                ? `0${jobs.filter(j => j.status === 'transit').length}` 
                : jobs.filter(j => j.status === 'transit').length}
            </span>
            <span className="text-[9px] font-bold text-slate-500 block mt-0.5">On highroad route</span>
          </div>

          <div className="bg-white rounded-2xl border border-slate-200/60 p-3.5 shadow-sm hover:shadow-md transition-shadow">
            <div className="flex items-center justify-between mb-1">
              <span className="text-slate-400 text-[10px] font-bold uppercase tracking-wider">Completed</span>
              <div className="p-1 rounded-lg bg-emerald-50 text-emerald-600">
                <CheckCircle className="w-4 h-4" />
              </div>
            </div>
            <span className="text-xl font-black text-slate-900">
              {jobs.filter(j => j.status === 'completed' || j.status === 'delivered').length < 10 
                ? `0${jobs.filter(j => j.status === 'completed' || j.status === 'delivered').length}` 
                : jobs.filter(j => j.status === 'completed' || j.status === 'delivered').length}
            </span>
            <span className="text-[9px] font-bold text-slate-500 block mt-0.5">Jobs completed</span>
          </div>
        </div>

        {/* Search Input Panel */}
        <div className="bg-white rounded-2xl border border-slate-200/80 shadow-sm p-4 mb-6">
          <div className="relative flex items-center bg-slate-50 border border-slate-200 focus-within:border-blue-600 focus-within:bg-white focus-within:ring-4 focus-within:ring-blue-500/5 rounded-xl transition-all duration-200">
            <Search className="absolute left-4 w-5 h-5 text-slate-400" />
            <input
              id="search-input"
              type="text"
              placeholder="Search supervisors, jobs, cities..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="w-full bg-transparent pl-12 pr-4 py-3.5 text-slate-800 outline-none text-sm placeholder-slate-400 font-medium"
            />
          </div>
        </div>

        {/* Secondary Navigation (Tabs) */}
        <div className="bg-white rounded-2xl border border-slate-200/60 p-2 mb-4 flex gap-1">
          {tabs.map(tab => (
            <button
              id={`tab-${tab.toLowerCase().replace(/\s+/g, '-')}`}
              key={tab}
              onClick={() => setActiveTab(tab)}
              className={`flex-1 text-center py-2.5 rounded-xl text-xs font-bold transition-all duration-200 cursor-pointer ${
                activeTab === tab 
                  ? 'bg-slate-900 text-white shadow-sm' 
                  : 'text-slate-500 hover:text-slate-800 hover:bg-slate-50'
              }`}
            >
              {tab}
            </button>
          ))}
        </div>

        {/* Action Chip Filters */}
        <div className="flex gap-2 mb-6 overflow-x-auto pb-1 scrollbar-none">
          {chips.map(chip => {
            const IconComponent = chip.icon;
            return (
              <button
                id={`chip-${chip.value.toLowerCase()}`}
                key={chip.value}
                onClick={() => setActiveChip(chip.value)}
                className={`flex items-center gap-2 px-4 py-2.5 rounded-xl text-xs font-extrabold shrink-0 cursor-pointer transition-all duration-200 ${
                  activeChip === chip.value
                    ? 'bg-blue-600 text-white shadow-md shadow-blue-500/10'
                    : 'bg-white text-slate-600 border border-slate-200 hover:bg-slate-50'
                }`}
              >
                <IconComponent className={`w-3.5 h-3.5 ${activeChip === chip.value ? 'text-white' : 'text-slate-500'}`} />
                <span>{chip.label}</span>
              </button>
            );
          })}
        </div>

        {/* Active Jobs Heading */}
        <div className="flex justify-between items-center mb-4 px-1">
          <div className="flex items-center gap-2">
            <h3 className="text-sm font-extrabold text-slate-900 uppercase tracking-wider">
              Assigned Consignments ({filteredJobs.length})
            </h3>
          </div>
          <span className="text-[10px] font-bold text-slate-400 uppercase tracking-widest">
            ● Realtime Updated
          </span>
        </div>

        {/* JobList Loop Grid */}
        {filteredJobs.length === 0 ? (
          <div className="bg-white rounded-2xl border border-slate-200/80 p-12 text-center shadow-sm">
            <div className="w-16 h-16 rounded-full bg-slate-50 flex items-center justify-center mx-auto mb-4 text-slate-300">
              <FileText className="w-8 h-8" />
            </div>
            <h4 className="text-slate-800 font-bold text-base">No active consignments</h4>
            <p className="text-slate-400 text-xs mt-1 max-w-xs mx-auto">
              There are no matching items for the current search queries or selected filters.
            </p>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {filteredJobs.map(job => (
              <div 
                id={`job-card-${job.id}`}
                key={job.id} 
                className="bg-white rounded-2xl border border-slate-200/80 p-5 shadow-sm hover:shadow-md transition-all duration-200 flex flex-col justify-between"
              >
                <div>
                  {/* Status Badge & Name */}
                  <div className="flex justify-between items-start mb-3">
                    <div className="flex flex-col">
                      <span className="text-[10px] font-bold text-slate-400 uppercase tracking-wider">
                        Consignment #{1000 + job.id}
                      </span>
                      <h3 className="text-base font-bold text-slate-900 tracking-tight leading-snug mt-0.5">
                        {job.name}
                      </h3>
                    </div>
                    <span className={`inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-[10px] font-extrabold uppercase tracking-wider ${
                      job.status === 'completed' || job.status === 'delivered'
                        ? 'bg-emerald-50 text-emerald-700 border border-emerald-100'
                        : job.status === 'transit' 
                        ? 'bg-blue-50 text-blue-700 border border-blue-100' 
                        : 'bg-amber-50 text-amber-700 border border-amber-100'
                    }`}>
                      <span className={`w-1.5 h-1.5 rounded-full ${
                        job.status === 'completed' || job.status === 'delivered'
                          ? 'bg-emerald-500'
                          : job.status === 'transit'
                          ? 'bg-blue-600'
                          : 'bg-amber-500'
                      }`} />
                      <span>{
                        job.status === 'completed' || job.status === 'delivered'
                          ? 'Completed'
                          : job.status === 'transit'
                          ? 'In Transit'
                          : 'Pending'
                      }</span>
                    </span>
                  </div>

                  {/* Route details */}
                  <div className="flex items-start gap-2.5 mb-3.5 bg-slate-50 p-2.5 rounded-xl border border-slate-100">
                    <MapPin className="w-4 h-4 text-blue-600 shrink-0 mt-0.5" />
                    <div className="flex flex-col">
                      <span className="text-[9px] font-extrabold text-slate-400 uppercase tracking-widest leading-none">Destination Route</span>
                      <span className="text-xs font-semibold text-slate-700 mt-1 line-clamp-1">{job.location}</span>
                    </div>
                  </div>

                  {/* Shipment Mode Badge */}
                  <div className="flex items-center gap-1.5 bg-slate-100 text-slate-700 px-3 py-1.5 rounded-lg text-xs font-bold w-fit mb-4">
                    <Truck className="w-3.5 h-3.5 text-slate-500" />
                    <span>{job.type}</span>
                  </div>

                  {/* Timings */}
                  <div className="grid grid-cols-2 gap-3 mb-5 bg-slate-50/50 p-3 rounded-xl border border-slate-100/50 text-xs">
                    <div className="flex items-center gap-2">
                      <Calendar className="w-4 h-4 text-slate-400" />
                      <span className="font-semibold text-slate-600">{job.date}</span>
                    </div>
                    <div className="flex items-center gap-2">
                      <Clock className="w-4 h-4 text-slate-400" />
                      <span className="font-semibold text-slate-600">{job.time}</span>
                    </div>
                  </div>
                </div>

                {/* Grid of Action Buttons */}
                <div className="flex gap-2.5">
                  <button
                    id={`job-${job.id}-pickup-btn`}
                    onClick={() => alert(`Showing Route parameters & Map tracking coordinates for ${job.name}`)}
                    className="flex-1 flex items-center justify-center gap-1.5 border border-slate-200 hover:bg-slate-50 text-slate-700 font-bold text-xs py-3 rounded-xl cursor-pointer transition-colors text-center"
                  >
                    <Map className="w-3.5 h-3.5" />
                    <span>View Map</span>
                  </button>
                  <button
                    id={`job-${job.id}-details-btn`}
                    onClick={() => onSelectJob(job)}
                    className="flex-1 flex items-center justify-center gap-1 bg-slate-900 hover:bg-slate-800 text-white font-bold text-xs py-3 rounded-xl shadow-sm cursor-pointer transition-all duration-200 text-center"
                  >
                    <span>Supervisor Hub</span>
                    <ArrowRight className="w-3.5 h-3.5" />
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}

        {/* Corporate Trust Banner */}
        <div className="text-center py-4 border-t border-slate-200/50 mt-10 text-[10px] text-slate-400 font-extrabold tracking-widest uppercase">
          🛡️ Secure Supervisor Gateway • PY-CORP-9.4
        </div>
      </div>

      {/* Floating style Corporate Bottom Nav Dock */}
      <nav className="fixed bottom-0 left-0 right-0 bg-white border-t border-slate-200/60 py-2.5 px-4 shadow-[0_-8px_30px_rgba(15,23,42,0.04)] z-40">
        <div className="max-w-md mx-auto flex justify-around items-center">
          <button
            id="nav-home"
            onClick={() => alert('Welcome to Home panel dashboard')}
            className="flex flex-col items-center gap-1 text-slate-400 hover:text-blue-600 transition-colors cursor-pointer"
          >
            <Home className="w-5 h-5" />
            <span className="text-[10px] font-bold">Home</span>
          </button>
          
          <button
            id="nav-earnings"
            onClick={() => alert('Supervisor Earnings & Incentives schedule is controlled by the HR team.')}
            className="flex flex-col items-center gap-1 text-slate-400 hover:text-blue-600 transition-colors cursor-pointer"
          >
            <Wallet className="w-5 h-5" />
            <span className="text-[10px] font-bold">Earnings</span>
          </button>
          
          <button
            id="nav-jobs"
            className="flex flex-col items-center gap-1 text-blue-600 relative cursor-pointer"
          >
            <Briefcase className="w-5 h-5" />
            <span className="text-[10px] font-bold">Consignments</span>
            <span className="absolute top-0 right-3.5 w-1.5 h-1.5 bg-blue-600 rounded-full animate-ping" />
          </button>

          <button
            id="nav-logout"
            onClick={() => setShowLogoutModal(true)}
            className="flex flex-col items-center gap-1 text-slate-400 hover:text-rose-600 transition-colors cursor-pointer"
          >
            <LogOut className="w-5 h-5" />
            <span className="text-[10px] font-bold">Logout</span>
          </button>
        </div>
      </nav>

      {/* Logout Confirmation Dialog Modal */}
      {showLogoutModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-900/60 backdrop-blur-sm animate-fade-in">
          <div className="bg-white rounded-2xl max-w-sm w-full p-6 shadow-2xl border border-slate-100 transform scale-100 transition-all duration-200">
            <h3 className="text-lg font-extrabold text-slate-900 mb-2.5">Terminate Session</h3>
            <p className="text-slate-500 text-sm mb-6 leading-relaxed">
              Are you sure you want to end your supervisor session and log out of the Packyatra corporate portal?
            </p>
            <div className="flex gap-3 justify-end">
              <button
                id="logout-cancel-btn"
                onClick={() => setShowLogoutModal(false)}
                className="px-5 py-2.5 rounded-xl border border-slate-200 text-slate-600 font-bold hover:bg-slate-50 cursor-pointer text-xs"
              >
                Cancel
              </button>
              <button
                id="logout-confirm-btn"
                onClick={handleLogoutConfirm}
                className="px-5 py-2.5 bg-rose-600 hover:bg-rose-700 text-white font-bold rounded-xl cursor-pointer text-xs shadow-md shadow-rose-500/10"
              >
                Log Out
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default LandingScreen;
