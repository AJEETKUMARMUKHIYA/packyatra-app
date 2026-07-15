import React, { useState } from 'react';
import { useAuth } from '../context/AuthContext';
import { Truck, Lock, User as UserIcon, Eye, EyeOff, Fingerprint, AlertCircle } from 'lucide-react';

interface LoginScreenProps {
  onLoginSuccess: () => void;
}

const LoginScreen: React.FC<LoginScreenProps> = ({ onLoginSuccess }) => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [loading, setLoading] = useState(false);
  const [errorMsg, setErrorMsg] = useState<string | null>(null);
  const { login } = useAuth();

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setErrorMsg(null);

    if (!email.trim()) {
      setErrorMsg('Please enter your email or username');
      return;
    }
    if (!password.trim()) {
      setErrorMsg('Please enter your password');
      return;
    }

    setLoading(true);
    const result = await login(email.trim(), password);
    setLoading(false);

    if (result.success) {
      onLoginSuccess();
    } else {
      setErrorMsg(result.error || 'Login failed. Please check your credentials.');
    }
  };

  return (
    <div className="relative min-h-screen w-full overflow-hidden flex items-center justify-center bg-gradient-to-b from-slate-50 via-slate-100 to-blue-50/50 p-4 select-none font-sans">
      {/* Decorative clean background patterns */}
      <div className="absolute top-[-20%] right-[-10%] w-[600px] h-[600px] rounded-full bg-blue-100/40 blur-[120px]" />
      <div className="absolute bottom-[-20%] left-[-10%] w-[600px] h-[600px] rounded-full bg-indigo-100/20 blur-[120px]" />
      
      {/* Subtle modern web grid lines */}
      <div className="absolute inset-0 bg-[linear-gradient(to_right,#e2e8f0_1px,transparent_1px),linear-gradient(to_bottom,#e2e8f0_1px,transparent_1px)] bg-[size:3rem_3rem] [mask-image:radial-gradient(ellipse_60%_50%_at_50%_50%,#000_80%,transparent_100%)] opacity-[0.25]" />

      {/* Main glass-morphic Login Card */}
      <div 
        id="login-card"
        className="relative w-full max-w-[440px] bg-white rounded-3xl border border-slate-200/80 shadow-[0_20px_50px_rgba(15,23,42,0.04)] p-6 sm:p-10 transition-all duration-300 hover:shadow-[0_24px_60px_rgba(15,23,42,0.08)]"
      >
        {/* Subtle top indicator bar */}
        <div className="absolute top-0 left-0 right-0 h-[4px] bg-gradient-to-r from-blue-600 to-indigo-600 rounded-t-3xl" />

        <form onSubmit={handleLogin} className="flex flex-col">
          {/* Header Section */}
          <div className="flex flex-col items-center text-center mb-8">
            <div className="flex items-center justify-center w-16 h-16 rounded-2xl bg-gradient-to-tr from-blue-600 to-indigo-600 shadow-md shadow-blue-600/10 mb-4 text-white">
              <Truck className="w-8 h-8 animate-[pulse_3s_infinite]" />
            </div>
            
            <h1 className="text-3xl font-black text-slate-900 tracking-tight mb-1.5 font-display">
              Packyatra
            </h1>
            <div className="inline-flex items-center gap-1.5 px-3 py-1 rounded-full bg-blue-50 border border-blue-100 text-blue-700 text-[10px] font-extrabold uppercase tracking-widest">
              <span className="w-1.5 h-1.5 rounded-full bg-blue-500 animate-pulse" />
              Supervisor Portal
            </div>
          </div>

          {errorMsg && (
            <div className="w-full bg-rose-50 text-rose-800 text-sm p-4 rounded-xl mb-6 border border-rose-100 flex items-start gap-3">
              <AlertCircle className="w-5 h-5 shrink-0 text-rose-600" />
              <span className="font-medium leading-relaxed">{errorMsg}</span>
            </div>
          )}

          {/* Email / User Input */}
          <div className="w-full mb-5">
            <label className="block text-xs font-bold uppercase tracking-wider text-slate-500 mb-2.5 ml-1">
              Username or Email
            </label>
            <div className="relative flex items-center bg-slate-50 border border-slate-200 focus-within:border-blue-600 focus-within:bg-white focus-within:ring-4 focus-within:ring-blue-500/5 rounded-xl transition-all duration-200">
              <UserIcon className="absolute left-4 w-5 h-5 text-slate-400" />
              <input
                id="email-input"
                type="text"
                placeholder="Enter supervisor username"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                className="w-full bg-transparent pl-12 pr-4 py-3.5 text-slate-900 outline-none text-sm placeholder-slate-400 font-medium"
                autoCapitalize="none"
              />
            </div>
          </div>

          {/* Password Input */}
          <div className="w-full mb-6">
            <div className="flex justify-between items-center mb-2.5 ml-1">
              <label className="block text-xs font-bold uppercase tracking-wider text-slate-500">
                Password
              </label>
            </div>
            <div className="relative flex items-center bg-slate-50 border border-slate-200 focus-within:border-blue-600 focus-within:bg-white focus-within:ring-4 focus-within:ring-blue-500/5 rounded-xl transition-all duration-200">
              <Lock className="absolute left-4 w-5 h-5 text-slate-400" />
              <input
                id="password-input"
                type={showPassword ? 'text' : 'password'}
                placeholder="Enter password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                className="w-full bg-transparent pl-12 pr-12 py-3.5 text-slate-900 outline-none text-sm placeholder-slate-400 font-medium"
              />
              <button
                id="password-toggle"
                type="button"
                onClick={() => setShowPassword(!showPassword)}
                className="absolute right-3 p-2 text-slate-400 hover:text-slate-600 transition-colors cursor-pointer"
              >
                {showPassword ? <EyeOff className="w-5 h-5" /> : <Eye className="w-5 h-5" />}
              </button>
            </div>
          </div>

          {/* Login Button */}
          <button
            id="login-submit-btn"
            type="submit"
            disabled={loading}
            className="w-full flex items-center justify-center py-4 bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-700 hover:to-indigo-700 text-white font-extrabold text-sm rounded-xl shadow-md shadow-blue-500/10 hover:shadow-lg transition-all duration-200 cursor-pointer disabled:opacity-80 disabled:cursor-not-allowed mb-4"
          >
            {loading ? (
              <div className="flex items-center gap-2">
                <svg className="animate-spin h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
                  <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
                  <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" />
                </svg>
                <span>Authenticating...</span>
              </div>
            ) : (
              <span>Sign In to Portal</span>
            )}
          </button>

          {/* Biometric pill */}
          <button
            id="biometric-login-btn"
            type="button"
            onClick={() => alert('Biometric hardware is not available in the web browser.')}
            className="flex items-center justify-center gap-2 bg-slate-50 hover:bg-slate-100 border border-slate-200 py-3 rounded-xl text-slate-600 text-xs font-bold transition-all duration-200 cursor-pointer"
          >
            <Fingerprint className="w-4 h-4 text-blue-600" />
            <span>Use Biometric Login</span>
          </button>

          {/* Decorative Footer */}
          <p className="text-center text-[10px] text-slate-400 font-extrabold tracking-widest uppercase mt-8">
            🛡️ Authorized Personnel Only
          </p>
        </form>
      </div>
    </div>
  );
};

export default LoginScreen;
