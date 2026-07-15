import React, { useRef, useState, useEffect } from 'react';
import { Job } from '../types';
import { 
  ArrowLeft, 
  LogOut, 
  Check, 
  X, 
  MapPin, 
  Calendar, 
  Clock, 
  Briefcase, 
  Layers, 
  Container, 
  Package, 
  Truck, 
  Camera, 
  Smartphone, 
  KeyRound, 
  ShieldAlert, 
  Trash2, 
  Plus, 
  Edit3, 
  QrCode, 
  CreditCard, 
  Home, 
  Star, 
  ThumbsUp, 
  ChevronLeft, 
  ChevronRight,
  Info,
  Compass,
  CheckCircle,
  FileText,
  ChevronDown,
  Upload,
  AlertTriangle,
  Image,
  Lock,
  MessageSquare,
  Phone
} from 'lucide-react';

interface JobDetailScreenProps {
  job: Job;
  onBack: () => void;
  onUpdateStatus: (status: string) => void;
  onLogout: () => void;
}

const JobDetailScreen: React.FC<JobDetailScreenProps> = ({ job, onBack, onUpdateStatus, onLogout }) => {
  const [currentStep, setCurrentStep] = useState(1);
  const totalSteps = 16;

  // ---------------------------------------------------------------------------
  // STEP-SPECIFIC STATES
  // ---------------------------------------------------------------------------
  
  // Step 2: OTP Verification
  const [otpSent, setOtpSent] = useState(false);
  const [otpCode, setOtpCode] = useState('');
  const [otpVerified, setOtpVerified] = useState(false);
  const [otpError, setOtpError] = useState('');

  // Step 3: Photos Before Packing (Group & Large items)
  const [beforePhotos, setBeforePhotos] = useState<string[]>([]);
  const [groupPhoto, setGroupPhoto] = useState<string | null>(null);
  const [largePhotos, setLargePhotos] = useState<(string | null)[]>([null, null, null]);
  const [selectedComments, setSelectedComments] = useState<string[]>([]);
  const [customComment, setCustomComment] = useState('');
  const [capturingGroup, setCapturingGroup] = useState(false);
  const [capturingLargeIdx, setCapturingLargeIdx] = useState<number | null>(null);
  const [groupDropdownOpen, setGroupDropdownOpen] = useState(true);
  const [largeDropdownOpen, setLargeDropdownOpen] = useState(true);

  // Step 4: Damage Photo
  const [hasDamage, setHasDamage] = useState<boolean | null>(null);
  const [damageDescription, setDamageDescription] = useState('');
  const [damageItemName, setDamageItemName] = useState('');
  const [damageLog, setDamageLog] = useState<Array<{ item: string; desc: string; photo: string }>>([
    { item: 'Shoe Rack Wooden', desc: 'Pre-existing deep scratch on the back panel', photo: 'https://images.unsplash.com/photo-1595428774223-ef52624120d2?w=150&auto=format&fit=crop&q=60&ixlib=rb-4.0.3' }
  ]);

  // Step 5: Packing Photo
  const [packingPhotos, setPackingPhotos] = useState<string[]>([
    'https://images.unsplash.com/photo-1538333581680-92890f845097?w=150&auto=format&fit=crop&q=60&ixlib=rb-4.0.3' // Bubble Wrapped Items
  ]);
  const [capturingPacking, setCapturingPacking] = useState(false);
  const [packingChecklist, setPackingChecklist] = useState({
    bubbleWrap: true,
    doubleBox: true,
    fragileTags: false,
    heavyItemsBottom: true,
  });

  // Step 6: Extra Inventory Add
  const [extraItems, setExtraItems] = useState<Array<{ name: string; qty: number; cost: number }>>([]);
  const [newExtraName, setNewExtraName] = useState('');
  const [newExtraQty, setNewExtraQty] = useState(1);

  // Step 7: Digital Signature
  const canvasRef = useRef<HTMLCanvasElement | null>(null);
  const [isDrawing, setIsDrawing] = useState(false);
  const [hasSigned, setHasSigned] = useState(false);
  const [signatureSaved, setSignatureSaved] = useState(false);

  // Supervisor Signature states for Step 7
  const supervisorCanvasRef = useRef<HTMLCanvasElement | null>(null);
  const [supervisorIsDrawing, setSupervisorIsDrawing] = useState(false);
  const [supervisorHasSigned, setSupervisorHasSigned] = useState(false);
  const [supervisorSignatureSaved, setSupervisorSignatureSaved] = useState(false);

  // Step 8: Payment Link
  const [paymentStatus, setPaymentStatus] = useState<'pending' | 'verifying' | 'success'>('pending');

  // Step 9: Warehouse Reach
  const [selectedWarehouse, setSelectedWarehouse] = useState('Nelamangala Central Hub');
  const [warehouseArrived, setWarehouseArrived] = useState(false);
  const [warehouseTime, setWarehouseTime] = useState('');

  // Step 10: Dispatch Done
  const [sealNo, setSealNo] = useState('SL-482910-X');
  const [dispatchConfirmed, setDispatchConfirmed] = useState(false);

  // Step 11: Reach Delivery Point
  const [destArrived, setDestArrived] = useState(false);
  const [destArrivalTime, setDestArrivalTime] = useState('');
  const [unloadedVerified, setUnloadedVerified] = useState(false);

  // Step 12: OTP for Delivery
  const [deliveryOtpSent, setDeliveryOtpSent] = useState(false);
  const [deliveryOtpCode, setDeliveryOtpCode] = useState('');
  const [deliveryOtpVerified, setDeliveryOtpVerified] = useState(false);
  const [deliveryOtpError, setDeliveryOtpError] = useState('');

  // Step 13: Unpacking Photo
  const [unpackingPhotos, setUnpackingPhotos] = useState<string[]>([]);
  const [capturingUnpacking, setCapturingUnpacking] = useState(false);

  // Step 14: Digital Delivery Signature
  const deliveryCanvasRef = useRef<HTMLCanvasElement | null>(null);
  const [deliveryIsDrawing, setDeliveryIsDrawing] = useState(false);
  const [deliveryHasSigned, setDeliveryHasSigned] = useState(false);
  const [deliverySignatureSaved, setDeliverySignatureSaved] = useState(false);

  // Step 15: Application - Rating
  const [appRating, setAppRating] = useState(5);
  const [appRatingComment, setAppRatingComment] = useState('');

  // Step 16: TIPS
  const [tipOption, setTipOption] = useState<'none' | '100' | '200' | '500' | 'custom'>('none');
  const [customTip, setCustomTip] = useState('');

  // Step 1 Interactive features (from uploaded screenshot)
  const [chatOpen, setChatOpen] = useState(false);
  const [chatMessages, setChatMessages] = useState<Array<{sender: 'user' | 'customer', text: string, time: string}>>([
    { sender: 'customer', text: 'Hi, is the truck arriving on time today?', time: '01:15 PM' },
    { sender: 'user', text: 'Yes sir, the vehicle is about to start soon.', time: '01:20 PM' }
  ]);
  const [messageText, setMessageText] = useState('');
  const [imageGalleryOpen, setImageGalleryOpen] = useState(false);
  const [approveRequestOpen, setApproveRequestOpen] = useState(false);
  const [callingSupport, setCallingSupport] = useState(false);
  const [extraItemsRequested, setExtraItemsRequested] = useState(false);

  // ---------------------------------------------------------------------------
  // SIGNATURE DRAWING CANVAS LOGIC
  // ---------------------------------------------------------------------------
  useEffect(() => {
    if (currentStep === 7) {
      if (canvasRef.current) {
        const canvas = canvasRef.current;
        const ctx = canvas.getContext('2d');
        if (ctx) {
          ctx.strokeStyle = '#1e293b';
          ctx.lineWidth = 3;
          ctx.lineCap = 'round';
        }
      }
      if (supervisorCanvasRef.current) {
        const canvas = supervisorCanvasRef.current;
        const ctx = canvas.getContext('2d');
        if (ctx) {
          ctx.strokeStyle = '#1e293b';
          ctx.lineWidth = 3;
          ctx.lineCap = 'round';
        }
      }
    }
  }, [currentStep, signatureSaved, supervisorSignatureSaved]);

  const getCoordinates = (e: React.MouseEvent<HTMLCanvasElement> | React.TouchEvent<HTMLCanvasElement>) => {
    const canvas = canvasRef.current;
    if (!canvas) return { x: 0, y: 0 };
    const rect = canvas.getBoundingClientRect();
    
    // Support mouse & touchscreen
    if ('touches' in e && e.touches.length > 0) {
      return {
        x: e.touches[0].clientX - rect.left,
        y: e.touches[0].clientY - rect.top
      };
    } else {
      const mouseEvent = e as React.MouseEvent<HTMLCanvasElement>;
      return {
        x: mouseEvent.clientX - rect.left,
        y: mouseEvent.clientY - rect.top
      };
    }
  };

  const startDrawing = (e: React.MouseEvent<HTMLCanvasElement> | React.TouchEvent<HTMLCanvasElement>) => {
    e.preventDefault();
    const canvas = canvasRef.current;
    if (!canvas) return;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    const coords = getCoordinates(e);
    ctx.beginPath();
    ctx.moveTo(coords.x, coords.y);
    setIsDrawing(true);
    setHasSigned(true);
  };

  const draw = (e: React.MouseEvent<HTMLCanvasElement> | React.TouchEvent<HTMLCanvasElement>) => {
    if (!isDrawing) return;
    e.preventDefault();
    const canvas = canvasRef.current;
    if (!canvas) return;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    const coords = getCoordinates(e);
    ctx.lineTo(coords.x, coords.y);
    ctx.stroke();
  };

  const stopDrawing = () => {
    setIsDrawing(false);
  };

  const clearCanvas = () => {
    const canvas = canvasRef.current;
    if (!canvas) return;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    setHasSigned(false);
    setSignatureSaved(false);
  };

  const saveSignature = () => {
    if (!hasSigned) return;
    setSignatureSaved(true);
    alert('Customer Digital Signature saved and locked successfully.');
  };

  // SUPERVISOR SIGNATURE CANVAS LOGIC
  const getSupervisorCoordinates = (e: React.MouseEvent<HTMLCanvasElement> | React.TouchEvent<HTMLCanvasElement>) => {
    const canvas = supervisorCanvasRef.current;
    if (!canvas) return { x: 0, y: 0 };
    const rect = canvas.getBoundingClientRect();
    
    if ('touches' in e && e.touches.length > 0) {
      return {
        x: e.touches[0].clientX - rect.left,
        y: e.touches[0].clientY - rect.top
      };
    } else {
      const mouseEvent = e as React.MouseEvent<HTMLCanvasElement>;
      return {
        x: mouseEvent.clientX - rect.left,
        y: mouseEvent.clientY - rect.top
      };
    }
  };

  const startSupervisorDrawing = (e: React.MouseEvent<HTMLCanvasElement> | React.TouchEvent<HTMLCanvasElement>) => {
    e.preventDefault();
    const canvas = supervisorCanvasRef.current;
    if (!canvas) return;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    const coords = getSupervisorCoordinates(e);
    ctx.beginPath();
    ctx.moveTo(coords.x, coords.y);
    setSupervisorIsDrawing(true);
    setSupervisorHasSigned(true);
  };

  const drawSupervisor = (e: React.MouseEvent<HTMLCanvasElement> | React.TouchEvent<HTMLCanvasElement>) => {
    if (!supervisorIsDrawing) return;
    e.preventDefault();
    const canvas = supervisorCanvasRef.current;
    if (!canvas) return;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    const coords = getSupervisorCoordinates(e);
    ctx.lineTo(coords.x, coords.y);
    ctx.stroke();
  };

  const stopSupervisorDrawing = () => {
    setSupervisorIsDrawing(false);
  };

  const clearSupervisorCanvas = () => {
    const canvas = supervisorCanvasRef.current;
    if (!canvas) return;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    setSupervisorHasSigned(false);
    setSupervisorSignatureSaved(false);
  };

  const saveSupervisorSignature = () => {
    if (!supervisorHasSigned) return;
    setSupervisorSignatureSaved(true);
    alert('Supervisor Digital Signature saved and locked successfully.');
  };

  // ---------------------------------------------------------------------------
  // DELIVERY SIGNATURE DRAWING CANVAS LOGIC
  // ---------------------------------------------------------------------------
  useEffect(() => {
    if (currentStep === 14 && deliveryCanvasRef.current) {
      const canvas = deliveryCanvasRef.current;
      const ctx = canvas.getContext('2d');
      if (ctx) {
        ctx.strokeStyle = '#1e293b';
        ctx.lineWidth = 3;
        ctx.lineCap = 'round';
      }
    }
  }, [currentStep]);

  const getDeliveryCoordinates = (e: React.MouseEvent<HTMLCanvasElement> | React.TouchEvent<HTMLCanvasElement>) => {
    const canvas = deliveryCanvasRef.current;
    if (!canvas) return { x: 0, y: 0 };
    const rect = canvas.getBoundingClientRect();
    if ('touches' in e && e.touches.length > 0) {
      return {
        x: e.touches[0].clientX - rect.left,
        y: e.touches[0].clientY - rect.top
      };
    } else {
      const mouseEvent = e as React.MouseEvent<HTMLCanvasElement>;
      return {
        x: mouseEvent.clientX - rect.left,
        y: mouseEvent.clientY - rect.top
      };
    }
  };

  const startDeliveryDrawing = (e: React.MouseEvent<HTMLCanvasElement> | React.TouchEvent<HTMLCanvasElement>) => {
    e.preventDefault();
    const canvas = deliveryCanvasRef.current;
    if (!canvas) return;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    const coords = getDeliveryCoordinates(e);
    ctx.beginPath();
    ctx.moveTo(coords.x, coords.y);
    setDeliveryIsDrawing(true);
    setDeliveryHasSigned(true);
  };

  const drawDelivery = (e: React.MouseEvent<HTMLCanvasElement> | React.TouchEvent<HTMLCanvasElement>) => {
    if (!deliveryIsDrawing) return;
    e.preventDefault();
    const canvas = deliveryCanvasRef.current;
    if (!canvas) return;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;

    const coords = getDeliveryCoordinates(e);
    ctx.lineTo(coords.x, coords.y);
    ctx.stroke();
  };

  const stopDeliveryDrawing = () => {
    setDeliveryIsDrawing(false);
  };

  const clearDeliveryCanvas = () => {
    const canvas = deliveryCanvasRef.current;
    if (!canvas) return;
    const ctx = canvas.getContext('2d');
    if (!ctx) return;
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    setDeliveryHasSigned(false);
    setDeliverySignatureSaved(false);
  };

  const saveDeliverySignature = () => {
    if (!deliveryHasSigned) return;
    setDeliverySignatureSaved(true);
    alert('Customer Digital Delivery Signature saved and locked successfully.');
  };

  // ---------------------------------------------------------------------------
  // ADDITIONAL INTERACTION SIMULATORS (STEPS 11-16)
  // ---------------------------------------------------------------------------
  const handleLogDestArrival = () => {
    setDestArrived(true);
    setDestArrivalTime(new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', second: '2-digit' }));
  };

  const handleSendDeliveryOtp = () => {
    setDeliveryOtpSent(true);
    setDeliveryOtpError('');
    alert('Simulated Verification: OTP sent to customer (+91 6364354615). CODE IS: 5812');
  };

  const handleVerifyDeliveryOtp = () => {
    if (deliveryOtpCode === '5812') {
      setDeliveryOtpVerified(true);
      setDeliveryOtpError('');
    } else {
      setDeliveryOtpError('Invalid OTP code. Please enter 5812');
    }
  };

  const handleAddUnpackingPhoto = () => {
    setCapturingUnpacking(true);
    setTimeout(() => {
      const mockPhotos = [
        'https://images.unsplash.com/photo-1584622650111-993a426fbf0a?w=200&auto=format&fit=crop&q=60', // Unpacked living room
        'https://images.unsplash.com/photo-1540518614846-7eded433c457?w=200&auto=format&fit=crop&q=60', // Unpacked bed
        'https://images.unsplash.com/photo-1556911220-e15b29be8c8f?w=200&auto=format&fit=crop&q=60'  // Kitchen/washing area
      ];
      const nextPhoto = mockPhotos[unpackingPhotos.length % mockPhotos.length];
      setUnpackingPhotos([...unpackingPhotos, nextPhoto]);
      setCapturingUnpacking(false);
    }, 800);
  };

  // ---------------------------------------------------------------------------
  // INTERACTION SIMULATORS
  // ---------------------------------------------------------------------------
  const handleSendOtp = () => {
    setOtpSent(true);
    setOtpError('');
    alert('Simulated Verification: OTP sent to customer (+91 6364354615). CODE IS: 4821');
  };

  const handleVerifyOtp = () => {
    if (otpCode === '4821') {
      setOtpVerified(true);
      setOtpError('');
      onUpdateStatus('transit'); // Mark job as transit in App state
    } else {
      setOtpError('Invalid OTP code. Please enter 4821');
    }
  };

  // Sync beforePhotos state with groupPhoto and largePhotos
  useEffect(() => {
    const activePhotos = [groupPhoto, ...largePhotos].filter((p): p is string => p !== null);
    setBeforePhotos(activePhotos);
  }, [groupPhoto, largePhotos]);

  const handleCaptureGroupPhoto = () => {
    setCapturingGroup(true);
    setTimeout(() => {
      setGroupPhoto('https://images.unsplash.com/photo-1522071820081-009f0129c71c?w=300&auto=format&fit=crop&q=60');
      setCapturingGroup(false);
    }, 600);
  };

  const handleCaptureLargePhoto = (index: number) => {
    if (!groupPhoto) {
      alert('Please upload a Group Image first!');
      return;
    }
    setCapturingLargeIdx(index);
    setTimeout(() => {
      const mockLargeItems = [
        'https://images.unsplash.com/photo-1593305841991-05c297ba4575?w=300&auto=format&fit=crop&q=60', // TV
        'https://images.unsplash.com/photo-1571175432220-1702b39a7593?w=300&auto=format&fit=crop&q=60', // Fridge
        'https://images.unsplash.com/photo-1626806787461-102c1bfaaea1?w=300&auto=format&fit=crop&q=60'  // Washing machine
      ];
      const updated = [...largePhotos];
      updated[index] = mockLargeItems[index] || mockLargeItems[0];
      setLargePhotos(updated);
      setCapturingLargeIdx(null);
    }, 600);
  };

  const toggleCommentTag = (tag: string) => {
    setSelectedComments(prev => 
      prev.includes(tag) ? prev.filter(t => t !== tag) : [...prev, tag]
    );
  };

  const handleAddDamageLog = () => {
    if (!damageItemName.trim() || !damageDescription.trim()) {
      alert('Please fill out both the Item Name and Description for the damage log.');
      return;
    }
    const damagePhotos = [
      'https://images.unsplash.com/photo-1562184560-a11b7cf7c166?w=150&auto=format&fit=crop&q=60&ixlib=rb-4.0.3',
      'https://images.unsplash.com/photo-1598300042247-d088f8ab3a91?w=150&auto=format&fit=crop&q=60&ixlib=rb-4.0.3'
    ];
    setDamageLog([
      ...damageLog,
      {
        item: damageItemName.trim(),
        desc: damageDescription.trim(),
        photo: damagePhotos[Math.floor(Math.random() * damagePhotos.length)]
      }
    ]);
    setDamageItemName('');
    setDamageDescription('');
  };

  const handleAddPackingPhoto = () => {
    setCapturingPacking(true);
    setTimeout(() => {
      const mockPhotos = [
        'https://images.unsplash.com/photo-1600585154526-990dced4db0d?w=150&auto=format&fit=crop&q=60&ixlib=rb-4.0.3', // Packaged box
        'https://images.unsplash.com/photo-1513151233558-d860c5398176?w=150&auto=format&fit=crop&q=60&ixlib=rb-4.0.3'  // Stack of cartoons
      ];
      const randomPhoto = mockPhotos[Math.floor(Math.random() * mockPhotos.length)];
      setPackingPhotos([...packingPhotos, randomPhoto]);
      setCapturingPacking(false);
    }, 1000);
  };

  const handleAddExtraInventory = () => {
    if (!newExtraName.trim()) {
      alert('Please enter an item name');
      return;
    }
    const costPerItem = 450;
    setExtraItems([
      ...extraItems,
      {
        name: newExtraName.trim(),
        qty: newExtraQty,
        cost: newExtraQty * costPerItem
      }
    ]);
    setNewExtraName('');
    setNewExtraQty(1);
  };

  const handleRemoveExtraItem = (index: number) => {
    setExtraItems(extraItems.filter((_, i) => i !== index));
  };

  const handleVerifyPayment = () => {
    setPaymentStatus('verifying');
    setTimeout(() => {
      setPaymentStatus('success');
    }, 1800);
  };

  const handleLogWarehouseArrival = () => {
    setWarehouseArrived(true);
    setWarehouseTime(new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', second: '2-digit' }));
  };

  const handleCompleteJob = () => {
    onUpdateStatus('completed');
    alert(`🎉 Success! Consignment for ${job.name} completed successfully.`);
    onBack();
  };

  // ---------------------------------------------------------------------------
  // STEP TITLES & DESCRIPTIONS FOR PROGRESS BAR
  // ---------------------------------------------------------------------------
  const stepsMetadata = [
    { number: 1, title: 'Job Details', desc: 'Verify scope of consignment work' },
    { number: 2, title: 'OTP Job Start', desc: 'Secure verification to start' },
    { number: 3, title: 'Photo Before Packing', desc: 'Log initial condition photos' },
    { number: 4, title: 'Damage Photo', desc: 'Document pre-existing damages' },
    { number: 5, title: 'Packing Photo', desc: 'Verify wrapping quality' },
    { number: 6, title: 'Extra Inventory Add', desc: 'Log unlisted items & adjust bill' },
    { number: 7, title: 'Digital Signature', desc: 'Collect customer endorsement' },
    { number: 8, title: 'Payment Link', desc: 'Process payment & generate receipt' },
    { number: 9, title: 'Warehouse Reach', desc: 'Log transition hub check-in' },
    { number: 10, title: 'Dispatch Done', desc: 'Lock seal & confirm dispatch' },
    { number: 11, title: 'Reach Delivery Point', desc: 'Log destination arrival & verify unloaded cargo' },
    { number: 12, title: 'OTP for Delivery', desc: 'Secure pin verification with recipient' },
    { number: 13, title: 'Unpacking Photo', desc: 'Log unpack condition of major appliances' },
    { number: 14, title: 'Digital Delivery Signature', desc: 'Customer signature on consignment handover' },
    { number: 15, title: 'Application - Rating', desc: 'Rate experience & log customer feedback' },
    { number: 16, title: 'TIPS', desc: 'Support crew with voluntary tips' }
  ];

  const activeStepMeta = stepsMetadata[currentStep - 1];

  // Helper validation to prevent proceeding if tasks aren't done
  const canProceed = () => {
    if (currentStep === 2 && !otpVerified) return false;
    if (currentStep === 3 && (!groupPhoto || largePhotos.some(p => p === null))) return false;
    if (currentStep === 4 && hasDamage === null) return false;
    if (currentStep === 7 && (!signatureSaved || !supervisorSignatureSaved)) return false;
    if (currentStep === 8 && paymentStatus !== 'success') return false;
    if (currentStep === 9 && !warehouseArrived) return false;
    if (currentStep === 10 && !dispatchConfirmed) return false;
    if (currentStep === 11 && (!destArrived || !unloadedVerified)) return false;
    if (currentStep === 12 && !deliveryOtpVerified) return false;
    if (currentStep === 13 && unpackingPhotos.length === 0) return false;
    if (currentStep === 14 && !deliverySignatureSaved) return false;
    return true;
  };

  return (
    <div className="min-h-screen w-full bg-white text-slate-800 flex flex-col justify-between font-sans select-none max-w-md mx-auto relative shadow-2xl border-x border-slate-200">
      
      {/* 1. STICKY MOBILE TOP BAR */}
      <div className="sticky top-0 z-40 bg-white/95 border-b border-slate-200 px-4 py-3 flex items-center justify-between">
        <div className="flex items-center gap-3">
          <button
            id="back-btn"
            onClick={onBack}
            className="w-10 h-10 rounded-xl bg-slate-50 hover:bg-slate-100 border border-slate-200 text-slate-800 flex items-center justify-center cursor-pointer transition-colors"
          >
            <ArrowLeft className="w-5 h-5" />
          </button>
          <div className="flex flex-col">
            <span className="text-[9px] font-black text-blue-600 uppercase tracking-widest leading-none">
              Active Consignment
            </span>
            <span className="text-sm font-bold text-slate-900 truncate max-w-[150px] mt-0.5">
              {job.name}
            </span>
          </div>
        </div>

        <div className="flex items-center gap-2">
          <div className="bg-slate-50 border border-slate-200 text-[10px] font-extrabold px-3 py-1.5 rounded-full text-slate-700">
            {currentStep}/11
          </div>
          <button
            id="logout-btn"
            onClick={onLogout}
            className="w-9 h-9 rounded-xl bg-rose-50 text-rose-600 border border-rose-200/60 flex items-center justify-center hover:bg-rose-100 transition-colors"
            title="Logout"
          >
            <LogOut className="w-4 h-4" />
          </button>
        </div>
      </div>

      {/* 2. DYNAMIC HORIZONTAL STEP INDICATOR */}
      <div className="bg-slate-50 px-4 py-3 border-b border-slate-200/80">
        <div className="flex justify-between items-center mb-1.5">
          <span className="text-[10px] font-extrabold text-blue-600 uppercase tracking-wider">
            {activeStepMeta.title}
          </span>
          <span className="text-[10px] font-bold text-slate-500">
            {Math.round((currentStep / totalSteps) * 100)}% Done
          </span>
        </div>
        
        {/* Progress bar gauge */}
        <div className="w-full bg-slate-200 h-1.5 rounded-full overflow-hidden flex gap-0.5">
          {Array.from({ length: totalSteps }).map((_, idx) => (
            <div 
              key={idx}
              className={`flex-1 h-full transition-all duration-300 ${
                idx + 1 < currentStep 
                  ? 'bg-emerald-500' 
                  : idx + 1 === currentStep 
                  ? 'bg-blue-600 animate-pulse' 
                  : 'bg-slate-200'
              }`}
            />
          ))}
        </div>
        <p className="text-[10px] text-slate-500 mt-1 font-medium leading-tight">
          {activeStepMeta.desc}
        </p>
      </div>

      {/* 3. SCROLLABLE STEP VIEW CONTENT PANEL */}
      <div className="flex-1 overflow-y-auto px-4 py-5 bg-white text-sm scrollbar-none text-slate-800">
        
        {/* STEP 1: JOB DETAILS VIEW */}
        {currentStep === 1 && (
          <div className="space-y-4 animate-fade-in text-slate-800">
            {/* B. CRN SECTION */}
            <div className="bg-slate-50 border border-slate-200 px-4 py-3 rounded-2xl flex flex-col gap-0.5">
              <span className="text-[9px] font-black text-slate-400 uppercase tracking-wider">CRN</span>
              <span className="text-sm font-black text-slate-800 tracking-wide">HSPM-14183719</span>
            </div>

            {/* C. CUSTOMER REQUIREMENT CARD */}
            <div className="bg-white border border-slate-200 p-4 rounded-3xl space-y-4 shadow-sm">
              <h3 className="text-[11px] font-black text-slate-500 tracking-widest uppercase border-b border-slate-100 pb-2">
                Customer Requirement
              </h3>

              <div className="grid grid-cols-2 gap-y-4 gap-x-2 text-xs">
                <div>
                  <span className="text-[9px] font-extrabold text-slate-400 block uppercase leading-none mb-1">Job date</span>
                  <span className="font-bold text-slate-800">17 February 2026</span>
                </div>
                <div>
                  <span className="text-[9px] font-extrabold text-slate-400 block uppercase leading-none mb-1">Time Slot</span>
                  <span className="font-bold text-slate-800">02:00 PM</span>
                </div>
                <div>
                  <span className="text-[9px] font-extrabold text-slate-400 block uppercase leading-none mb-1">Movement Type</span>
                  <span className="font-bold text-slate-800">Inter City Movement</span>
                </div>
                <div>
                  <span className="text-[9px] font-extrabold text-slate-400 block uppercase leading-none mb-1">Packaging Type</span>
                  <span className="font-bold text-slate-800">Premium Packaging</span>
                </div>
                <div>
                  <span className="text-[9px] font-extrabold text-slate-400 block uppercase leading-none mb-1">Rent Cartons</span>
                  <span className="font-bold text-slate-800">15</span>
                </div>
                <div>
                  <span className="text-[9px] font-extrabold text-slate-400 block uppercase leading-none mb-1">Load Type</span>
                  <span className="font-bold text-slate-800">Separate Load</span>
                </div>
                <div>
                  <span className="text-[9px] font-extrabold text-slate-400 block uppercase leading-none mb-1">CFT Volume</span>
                  <span className="font-bold text-slate-800">329</span>
                </div>
                <div>
                  <span className="text-[9px] font-extrabold text-slate-400 block uppercase leading-none mb-1">Tentative Delivery</span>
                  <span className="font-bold text-slate-800">19 February 2026</span>
                </div>
                <div className="col-span-2 pt-3 border-t border-slate-100 flex items-center justify-between">
                  <div>
                    <span className="text-[9px] font-extrabold text-slate-400 block uppercase leading-none mb-1">Your Earning</span>
                    <span className="text-sm font-black text-[#1e8875]">₹ 16,560</span>
                  </div>
                </div>
              </div>
            </div>

            {/* D. INVENTORY DETAILS */}
            <div className="bg-white border border-slate-200 p-4 rounded-3xl space-y-3.5 shadow-sm">
              <h3 className="text-[11px] font-black text-slate-500 tracking-widest uppercase border-b border-slate-100 pb-2">
                Inventory Details
              </h3>

              <div className="space-y-2 max-h-[280px] overflow-y-auto pr-1 scrollbar-none">
                {[
                  { name: 'Small Pots', qty: 6 },
                  { name: 'Washing Machine <6.9kg', qty: 1 },
                  { name: 'Mixer Grinder', qty: 1 },
                  { name: 'Book Shelf Medium', qty: 1 },
                  { name: 'Shoe Rack Wooden', qty: 1 },
                  { name: 'Suitcase Small (7kg)', qty: 5 },
                  { name: 'Chest of Drawers Small', qty: 1 },
                  { name: 'Plastic Chair', qty: 5 },
                  { name: 'NoBroker Carton', qty: 15 },
                  { name: 'Fridge Single Door', qty: 1 },
                  { name: 'LCD/LED TV 40" & Below', qty: 1 },
                  { name: 'Water Purifier', qty: 1 },
                  { name: 'Water Drum', qty: 1 },
                  { name: 'Gas Stove / Hob', qty: 1 },
                  { name: 'Prayer Unit/Mandir', qty: 1 },
                  { name: 'Steel Almirah Medium', qty: 2 },
                  { name: 'Double Door Wardrobe', qty: 1 },
                  { name: 'Center Table', qty: 1 },
                  { name: 'Kitchen Rack', qty: 1 },
                  { name: 'Suitcase Medium (15kg)', qty: 2 },
                  { name: 'Step Ladder', qty: 1 },
                ].map((item, idx) => (
                  <div key={idx} className="flex justify-between items-center py-1.5 text-xs text-slate-700 border-b border-slate-50 last:border-0">
                    <span className="font-semibold text-slate-600">{item.name}</span>
                    <span className="font-extrabold text-slate-900 bg-slate-50 px-2 py-0.5 rounded-md">({item.qty})</span>
                  </div>
                ))}
              </div>
            </div>

            {/* E. MOVING FROM */}
            <div className="bg-white border border-slate-200 p-4 rounded-3xl space-y-3 shadow-sm">
              <div className="flex justify-between items-start gap-4">
                <div className="space-y-1">
                  <span className="text-[9px] font-black text-amber-600 uppercase tracking-widest block">Moving From</span>
                  <p className="text-[11px] text-slate-700 font-medium leading-relaxed">
                    g 14, First floor, First cross , NR colony, Munugesh palya, near maha veeri enterprises, Bangalore, Karnataka, India
                  </p>
                </div>
                <a 
                  href="https://www.google.com/maps/dir/?api=1&destination=g+14,+First+floor,+First+cross+,+NR+colony,+Munugesh+palya,+near+maha+veeri+enterprises,+Bangalore,+Karnataka,+India"
                  target="_blank"
                  rel="noopener noreferrer"
                  className="w-8 h-8 rounded-full bg-slate-50 border border-slate-200 flex items-center justify-center hover:bg-slate-100 text-[#1e8875] shrink-0"
                >
                  <Compass className="w-4 h-4" />
                </a>
              </div>
              <div className="grid grid-cols-2 gap-2 pt-2 border-t border-slate-100 text-xs">
                <div>
                  <span className="text-[9px] font-extrabold text-slate-400 block uppercase leading-none mb-1">Service Lift</span>
                  <span className="font-bold text-slate-700">No</span>
                </div>
                <div>
                  <span className="text-[9px] font-extrabold text-slate-400 block uppercase leading-none mb-1">Floor</span>
                  <span className="font-bold text-slate-700">1</span>
                </div>
              </div>
            </div>

            {/* F. MOVING TO */}
            <div className="bg-white border border-slate-200 p-4 rounded-3xl space-y-3 shadow-sm">
              <div className="flex justify-between items-start gap-4">
                <div className="space-y-1">
                  <span className="text-[9px] font-black text-emerald-600 uppercase tracking-widest block">Moving To</span>
                  <p className="text-[11px] text-slate-700 font-medium leading-relaxed">
                    2/5, ground floor, Agraharam, Eachangudi post, via thiruvaiyru, thanjavur district, Tamil Nadu, India
                  </p>
                </div>
                <a 
                  href="https://www.google.com/maps/dir/?api=1&origin=g+14,+First+floor,+First+cross+,+NR+colony,+Munugesh+palya,+near+maha+veeri+enterprises,+Bangalore,+Karnataka,+India&destination=2/5,+ground+floor,+Agraharam,+Eachangudi+post,+via+thiruvaiyru,+thanjavur+district,+Tamil+Nadu,+India"
                  target="_blank"
                  rel="noopener noreferrer"
                  className="w-8 h-8 rounded-full bg-slate-50 border border-slate-200 flex items-center justify-center hover:bg-slate-100 text-[#1e8875] shrink-0"
                >
                  <Compass className="w-4 h-4" />
                </a>
              </div>
              <div className="grid grid-cols-2 gap-2 pt-2 border-t border-slate-100 text-xs">
                <div>
                  <span className="text-[9px] font-extrabold text-slate-400 block uppercase leading-none mb-1">Service Lift</span>
                  <span className="font-bold text-slate-700">No</span>
                </div>
                <div>
                  <span className="text-[9px] font-extrabold text-slate-400 block uppercase leading-none mb-1">Floor</span>
                  <span className="font-bold text-slate-700">NA</span>
                </div>
              </div>
            </div>

            {/* G. VIEW UPLOADED IMAGES */}
            <button
              onClick={() => setImageGalleryOpen(true)}
              className="w-full py-2.5 border border-[#1e8875] text-[#1e8875] hover:bg-teal-50 font-bold text-xs uppercase tracking-widest rounded-xl transition-all cursor-pointer flex items-center justify-center gap-2"
            >
              <Image className="w-4 h-4" />
              <span>View Uploaded Images</span>
            </button>

            {/* H. APPROVE OR REQUEST BUTTON ROW */}
            <button
              onClick={() => setApproveRequestOpen(true)}
              className="w-full p-4 bg-white border border-slate-200 rounded-2xl flex items-center justify-between text-xs font-bold hover:bg-slate-50 transition-colors cursor-pointer"
            >
              <div className="flex items-center gap-2.5">
                <CheckCircle className="w-4 h-4 text-[#1e8875]" />
                <span className="text-slate-700">Approve or request changes</span>
              </div>
              <ChevronRight className="w-4 h-4 text-slate-400" />
            </button>
          </div>
        )}

        {/* STEP 2: OTP JOB START */}
        {currentStep === 2 && (
          <div className="space-y-4 animate-fade-in text-slate-800">
            <div className="bg-slate-50 border border-slate-200 p-5 rounded-3xl text-center space-y-4">
              <div className="w-14 h-14 bg-blue-50 border border-blue-200 rounded-2xl flex items-center justify-center mx-auto text-blue-600">
                <KeyRound className="w-7 h-7" />
              </div>
              
              <div className="space-y-1">
                <h3 className="text-base font-bold text-slate-900">Customer Verification OTP</h3>
                <p className="text-xs text-slate-500 max-w-xs mx-auto leading-relaxed">
                  To officially register job commencement, trigger the verification code to customer phone <strong className="text-slate-700">+91 6364354615</strong>.
                </p>
              </div>

              {!otpSent ? (
                <button
                  onClick={handleSendOtp}
                  className="w-full py-3 bg-blue-600 hover:bg-blue-700 text-white font-extrabold text-xs uppercase tracking-widest rounded-xl transition-all shadow-md shadow-blue-500/10 cursor-pointer"
                >
                  📡 Trigger Security OTP
                </button>
              ) : (
                <div className="space-y-4 pt-2">
                  <div className="bg-amber-50 border border-amber-200 p-2.5 rounded-xl text-center">
                    <span className="text-[10px] text-amber-700 font-extrabold uppercase tracking-widest block">Simulated Device SMS</span>
                    <span className="text-xs font-bold text-slate-800 mt-1 block">Customer received OTP: <strong className="text-slate-900 text-base tracking-widest">4821</strong></span>
                  </div>

                  <div className="space-y-2">
                    <label className="text-[10px] font-extrabold text-slate-500 uppercase tracking-widest block text-left">Enter 4-Digit Code</label>
                    <div className="flex gap-2 justify-center">
                      <input
                        type="text"
                        maxLength={4}
                        placeholder="••••"
                        value={otpCode}
                        onChange={(e) => setOtpCode(e.target.value.replace(/\D/g, ''))}
                        className="w-36 bg-white border border-slate-300 text-slate-900 font-extrabold text-lg text-center tracking-[0.5em] py-2.5 rounded-xl outline-none focus:border-blue-500"
                      />
                    </div>
                  </div>

                  {otpError && (
                    <p className="text-[11px] text-rose-600 font-bold">{otpError}</p>
                  )}

                  {!otpVerified ? (
                    <button
                      onClick={handleVerifyOtp}
                      className="w-full py-3 bg-slate-900 hover:bg-slate-800 text-white font-black text-xs uppercase tracking-widest rounded-xl transition-all cursor-pointer"
                    >
                      ✓ Verify & Unlock Job
                    </button>
                  ) : (
                    <div className="bg-emerald-50 border border-emerald-200 p-3 rounded-xl flex items-center justify-center gap-2 text-emerald-700">
                      <Check className="w-4 h-4" />
                      <span className="text-xs font-extrabold uppercase tracking-wider">Job Unlocked & Started</span>
                    </div>
                  )}
                </div>
              )}
            </div>

            <div className="bg-slate-50 border border-slate-200 p-4 rounded-2xl text-xs text-slate-500 space-y-2">
              <p className="font-extrabold text-slate-700 uppercase text-[9px] tracking-wider">🔒 Security Note</p>
              <p className="leading-relaxed">
                Packyatra corporate safety standard requires supervisors to verify the on-site presence of customer before commencing wrapping. This generates real-time telemetry timestamps for insurance claims.
              </p>
            </div>
          </div>
        )}

        {/* STEP 3: PHOTO BEFORE PACKING */}
        {currentStep === 3 && (
          <div className="space-y-4 animate-fade-in text-slate-800">
            {/* Warning Banner */}
            <div className="bg-rose-50 border border-rose-200/60 p-3.5 rounded-2xl flex items-start gap-2.5">
              <Info className="w-5 h-5 text-rose-500 shrink-0 mt-0.5" />
              <p className="text-xs text-rose-700 leading-normal font-semibold">
                Make sure to click all the images. Missing images will make the partner responsible for reported damages
              </p>
            </div>

            {/* SECTION 1: TAKE GROUP IMAGE */}
            <div className="bg-slate-50 border border-slate-200 rounded-2xl overflow-hidden shadow-sm">
              <button 
                onClick={() => setGroupDropdownOpen(!groupDropdownOpen)}
                className="w-full px-4 py-3.5 flex items-center justify-between bg-white border-b border-slate-100 hover:bg-slate-50/50 transition-colors"
              >
                <span className="text-xs font-black text-slate-800 uppercase tracking-wider">
                  Take Group Image
                </span>
                <ChevronDown className={`w-4 h-4 text-[#1e8875] transition-transform duration-300 ${groupDropdownOpen ? 'rotate-180' : ''}`} />
              </button>

              {groupDropdownOpen && (
                <div className="p-4 space-y-3 bg-white animate-fade-in">
                  <p className="text-xs text-slate-500 font-medium">
                    Please take photograph of entire team present at location.
                  </p>

                  <div className="flex justify-center py-2">
                    {groupPhoto ? (
                      <div className="relative w-full max-w-xs aspect-video rounded-xl overflow-hidden border border-slate-200 shadow-sm group">
                        <img src={groupPhoto} alt="Team Group" className="w-full h-full object-cover" />
                        <button
                          onClick={() => setGroupPhoto(null)}
                          className="absolute top-2 right-2 w-7 h-7 bg-rose-50 border border-rose-200 text-rose-600 rounded-full flex items-center justify-center cursor-pointer hover:bg-rose-100 shadow-md transition-colors"
                        >
                          <X className="w-4 h-4" />
                        </button>
                      </div>
                    ) : (
                      <button
                        onClick={handleCaptureGroupPhoto}
                        disabled={capturingGroup}
                        className="w-full max-w-xs aspect-video bg-slate-50 hover:bg-slate-100/80 border-2 border-dashed border-slate-200 rounded-xl flex flex-col items-center justify-center gap-2 p-4 cursor-pointer group transition-all"
                      >
                        {capturingGroup ? (
                          <div className="flex flex-col items-center gap-2">
                            <div className="w-6 h-6 border-2 border-teal-600 border-t-transparent rounded-full animate-spin" />
                            <span className="text-[10px] text-slate-400 font-bold">Capturing...</span>
                          </div>
                        ) : (
                          <>
                            <Image className="w-8 h-8 text-slate-400 group-hover:scale-105 transition-transform" />
                            <span className="text-[10px] font-extrabold uppercase tracking-widest bg-[#1e8875] hover:bg-[#156e5e] text-white px-3 py-1.5 rounded-lg shadow-sm flex items-center gap-1">
                              <Upload className="w-3 h-3" /> Upload ⬆
                            </span>
                          </>
                        )}
                      </button>
                    )}
                  </div>
                </div>
              )}
            </div>

            {/* SECTION 2: TAKE LARGE ELECTRONIC ITEMS IMAGE */}
            <div className="bg-slate-50 border border-slate-200 rounded-2xl overflow-hidden shadow-sm relative">
              <button 
                onClick={() => setLargeDropdownOpen(!largeDropdownOpen)}
                className="w-full px-4 py-3.5 flex items-center justify-between bg-white border-b border-slate-100 hover:bg-slate-50/50 transition-colors"
              >
                <span className="text-xs font-black text-slate-800 uppercase tracking-wider">
                  Take Large Electronic Items Image
                </span>
                <ChevronDown className={`w-4 h-4 text-[#1e8875] transition-transform duration-300 ${largeDropdownOpen ? 'rotate-180' : ''}`} />
              </button>

              {largeDropdownOpen && (
                <div className="p-4 space-y-4 bg-white animate-fade-in relative">
                  
                  {/* Lock Overlay if Group Photo is missing */}
                  {!groupPhoto && (
                    <div className="absolute inset-0 bg-white/95 backdrop-blur-xs z-20 flex flex-col items-center justify-center text-center p-6 space-y-2">
                      <div className="w-12 h-12 bg-slate-100 rounded-full flex items-center justify-center text-slate-400 shadow-inner">
                        <Lock className="w-5 h-5" />
                      </div>
                      <h4 className="text-xs font-bold text-slate-800 uppercase tracking-wider">Upload Group Image First</h4>
                      <p className="text-[10.5px] text-slate-500 max-w-xs leading-relaxed font-medium">
                        To comply with Indian logistics safety standards, you must log the team group photograph before capturing item inventories.
                      </p>
                    </div>
                  )}

                  <div className="flex justify-between items-center">
                    <span className="text-xs font-bold text-slate-500">
                      No. of images needed - <strong className="text-rose-600 font-extrabold">3</strong>
                    </span>
                  </div>

                  <div className="bg-slate-50 border border-slate-200 p-2.5 rounded-xl text-center text-[10.5px] text-slate-600 font-semibold leading-relaxed">
                    LCD/LED TV 40" & Below, Fridge Single Door, Washing Machine &lt;6.9kg
                  </div>

                  {/* Grid of 3 item slots */}
                  <div className="grid grid-cols-3 gap-2.5">
                    {largePhotos.map((photo, i) => {
                      const labels = ["TV Photo", "Fridge Photo", "Washing Machine"];
                      return (
                        <div key={i} className="flex flex-col items-center gap-1.5">
                          <span className="text-[9px] font-bold text-slate-400 uppercase tracking-wider truncate w-full text-center">
                            {labels[i]}
                          </span>
                          
                          {photo ? (
                            <div className="relative w-full aspect-square bg-slate-50 rounded-xl overflow-hidden border border-slate-200 shadow-sm group">
                              <img src={photo} alt={labels[i]} className="w-full h-full object-cover" />
                              <button
                                onClick={() => {
                                  const updated = [...largePhotos];
                                  updated[i] = null;
                                  setLargePhotos(updated);
                                }}
                                className="absolute top-1 right-1 w-5.5 h-5.5 bg-rose-50 border border-rose-200 text-rose-600 rounded-full flex items-center justify-center cursor-pointer hover:bg-rose-100 shadow"
                              >
                                <X className="w-3 h-3" />
                              </button>
                            </div>
                          ) : (
                            <button
                              onClick={() => handleCaptureLargePhoto(i)}
                              disabled={capturingLargeIdx !== null}
                              className="w-full aspect-square bg-slate-50 hover:bg-slate-100/80 border-2 border-dashed border-slate-200 rounded-xl flex flex-col items-center justify-center gap-1 p-2 cursor-pointer transition-colors"
                            >
                              {capturingLargeIdx === i ? (
                                <div className="w-4 h-4 border-2 border-teal-600 border-t-transparent rounded-full animate-spin" />
                              ) : (
                                <>
                                  <Image className="w-5 h-5 text-slate-300" />
                                  <span className="text-[8px] font-extrabold bg-[#1e8875] text-white px-1.5 py-0.5 rounded uppercase">
                                    Upload ⬆
                                  </span>
                                </>
                              )}
                            </button>
                          )}
                        </div>
                      );
                    })}
                  </div>
                </div>
              )}
            </div>

            {/* SECTION 3: ADD COMMENT */}
            <div className="bg-slate-50 border border-slate-200 p-4 rounded-3xl space-y-3 shadow-sm">
              <span className="text-[10px] font-extrabold text-slate-500 uppercase tracking-widest block">
                Add Comment
              </span>
              
              <div className="flex flex-wrap gap-2">
                {["Damage foot", "Scratches", "Tears & Rips", "Dent", "Satins & Spills"].map((tag) => {
                  const isActive = selectedComments.includes(tag);
                  return (
                    <button
                      key={tag}
                      onClick={() => toggleCommentTag(tag)}
                      className={`text-[10.5px] font-bold px-3 py-1.5 rounded-full border transition-all cursor-pointer ${
                        isActive 
                          ? 'bg-[#e6f4f1] border-[#1e8875] text-[#1e8875] shadow-xs' 
                          : 'bg-white border-slate-200 text-slate-600 hover:bg-slate-50'
                      }`}
                    >
                      {tag} {isActive && "✓"}
                    </button>
                  );
                })}
              </div>

              <input
                type="text"
                placeholder="Write additional specifications / condition logs..."
                value={customComment}
                onChange={(e) => setCustomComment(e.target.value)}
                className="w-full bg-white border border-slate-200 text-slate-800 rounded-xl px-3 py-2 text-xs outline-none focus:border-teal-500 font-medium placeholder-slate-400"
              />
            </div>

            {/* Info Protocol Note */}
            <div className="bg-blue-50 border border-blue-100 p-4 rounded-2xl text-xs text-slate-500 leading-relaxed shadow-xs flex items-start gap-2.5">
              <Info className="w-4 h-4 text-blue-600 shrink-0 mt-0.5" />
              <p>
                Take crisp, unobstructed photographs of on-site crew and major electronics before bubble wrapping. This establishes on-site telemetry proof and safeguards our crew from pre-existing claim disputes.
              </p>
            </div>
          </div>
        )}

        {/* STEP 4: DAMAGE PHOTO */}
        {currentStep === 4 && (
          <div className="space-y-4 animate-fade-in text-slate-800">
            <div className="bg-slate-50 border border-slate-200 p-4 rounded-3xl space-y-4">
              <h3 className="text-xs font-extrabold text-slate-700 uppercase tracking-widest border-b border-slate-200 pb-2">
                Pre-existing Damages Declaration
              </h3>

              <div className="space-y-2">
                <label className="text-[10px] font-extrabold text-slate-500 uppercase tracking-widest block">Are there any pre-existing damages on goods?</label>
                <div className="grid grid-cols-2 gap-2">
                  <button
                    onClick={() => setHasDamage(true)}
                    className={`py-2.5 rounded-xl font-bold text-xs border transition-all cursor-pointer ${
                      hasDamage === true
                        ? 'bg-rose-50 border-rose-200 text-rose-700 shadow-sm'
                        : 'bg-white border-slate-200 text-slate-500 hover:text-slate-800 hover:bg-slate-50'
                    }`}
                  >
                    Yes, log damages
                  </button>
                  <button
                    onClick={() => {
                      setHasDamage(false);
                      setDamageLog([]); // Clear out logged ones if they say no
                    }}
                    className={`py-2.5 rounded-xl font-bold text-xs border transition-all cursor-pointer ${
                      hasDamage === false
                        ? 'bg-emerald-50 border-emerald-200 text-emerald-700 shadow-sm'
                        : 'bg-white border-slate-200 text-slate-500 hover:text-slate-800 hover:bg-slate-50'
                    }`}
                  >
                    No, everything pristine
                  </button>
                </div>
              </div>

              {hasDamage === true && (
                <div className="space-y-3 pt-2 border-t border-slate-200">
                  <div className="space-y-2 bg-slate-50 p-3 rounded-xl border border-slate-200">
                    <span className="text-[10px] font-extrabold text-rose-600 block uppercase">Log Pre-existing Damage Form</span>
                    
                    <input
                      type="text"
                      placeholder="Item Name (e.g. Fridge Single Door)"
                      value={damageItemName}
                      onChange={(e) => setDamageItemName(e.target.value)}
                      className="w-full bg-white border border-slate-300 text-slate-800 rounded-lg px-3 py-2 text-xs outline-none focus:border-blue-500"
                    />

                    <textarea
                      placeholder="Damage Description (e.g. Dent on handle, glass hairline crack)"
                      value={damageDescription}
                      onChange={(e) => setDamageDescription(e.target.value)}
                      rows={2}
                      className="w-full bg-white border border-slate-300 text-slate-800 rounded-lg px-3 py-2 text-xs outline-none focus:border-blue-500 resize-none"
                    />

                    <button
                      onClick={handleAddDamageLog}
                      className="w-full py-2 bg-rose-50 border border-rose-200 text-rose-600 font-bold text-xs rounded-lg flex items-center justify-center gap-1 hover:bg-rose-100 transition-all cursor-pointer"
                    >
                      <Plus className="w-3.5 h-3.5" />
                      <span>Attach & Register Damage</span>
                    </button>
                  </div>

                  {damageLog.length > 0 && (
                    <div className="space-y-2">
                      <span className="text-[10px] font-extrabold text-slate-500 uppercase tracking-widest block">Logged Damage Registry ({damageLog.length})</span>
                      <div className="space-y-2">
                        {damageLog.map((d, i) => (
                          <div key={i} className="flex gap-2.5 bg-white border border-slate-200 p-2.5 rounded-xl items-center justify-between text-xs">
                            <div className="flex gap-2.5 items-center">
                              <img src={d.photo} className="w-10 h-10 rounded-lg object-cover border border-slate-200" />
                              <div className="flex flex-col">
                                <span className="font-bold text-rose-700">{d.item}</span>
                                <span className="text-[10px] text-slate-500 leading-tight">{d.desc}</span>
                              </div>
                            </div>
                            <button
                              onClick={() => setDamageLog(damageLog.filter((_, idx) => idx !== i))}
                              className="text-slate-400 hover:text-rose-600 p-1 cursor-pointer"
                            >
                              <Trash2 className="w-4 h-4" />
                            </button>
                          </div>
                        ))}
                      </div>
                    </div>
                  )}
                </div>
              )}
            </div>

            <div className="bg-slate-50 border border-slate-200 p-4 rounded-2xl text-xs text-slate-500 leading-relaxed">
              <ShieldAlert className="w-4 h-4 text-amber-600 inline mr-1.5 align-text-bottom" />
              If you log pre-existing damage, the customer's signature in Step 7 certifies liability waiver for those items. Always declare truthfully.
            </div>
          </div>
        )}

        {/* STEP 5: PACKING PHOTO */}
        {currentStep === 5 && (
          <div className="space-y-4 animate-fade-in text-slate-800">
            <div className="bg-slate-50 border border-slate-200 p-4 rounded-3xl space-y-4">
              <div className="flex justify-between items-center border-b border-slate-200 pb-2">
                <h3 className="text-xs font-extrabold text-slate-700 uppercase tracking-widest">
                  Wrapping & Packing Gallery ({packingPhotos.length})
                </h3>
              </div>

              <div className="grid grid-cols-3 gap-2">
                {packingPhotos.map((p, i) => (
                  <div key={i} className="relative aspect-square bg-white rounded-xl overflow-hidden group border border-slate-200">
                    <img src={p} alt="Packing photo" className="w-full h-full object-cover" />
                    <button
                      onClick={() => setPackingPhotos(packingPhotos.filter((_, idx) => idx !== i))}
                      className="absolute top-1 right-1 w-6 h-6 bg-rose-50 border border-rose-200 text-rose-600 rounded-lg flex items-center justify-center cursor-pointer hover:bg-rose-100"
                    >
                      <X className="w-3.5 h-3.5" />
                    </button>
                  </div>
                ))}
              </div>

              <button
                onClick={handleAddPackingPhoto}
                disabled={capturingPacking}
                className="w-full py-3 bg-blue-50 hover:bg-blue-100 text-blue-600 border border-blue-200 font-bold text-xs uppercase tracking-widest rounded-xl transition-all flex items-center justify-center gap-2 cursor-pointer"
              >
                <Camera className="w-4 h-4" />
                <span>{capturingPacking ? 'Logging image...' : '📸 Snap Bubble Wrap / Boxed Goods'}</span>
              </button>
            </div>

            {/* Checklist */}
            <div className="bg-slate-50 border border-slate-200 p-4 rounded-3xl space-y-3">
              <h4 className="text-[10px] font-black text-slate-500 uppercase tracking-widest border-b border-slate-200 pb-2">
                Packaging Protocol Checklist
              </h4>
              <div className="space-y-2.5">
                <label className="flex items-center gap-3 text-xs text-slate-600 cursor-pointer">
                  <input
                    type="checkbox"
                    checked={packingChecklist.bubbleWrap}
                    onChange={(e) => setPackingChecklist({...packingChecklist, bubbleWrap: e.target.checked})}
                    className="w-4.5 h-4.5 rounded border-slate-300 bg-white text-blue-600 accent-blue-600 focus:ring-0 focus:ring-offset-0 cursor-pointer"
                  />
                  <span>Bubble wrap applied to all glass/fragile items</span>
                </label>

                <label className="flex items-center gap-3 text-xs text-slate-600 cursor-pointer">
                  <input
                    type="checkbox"
                    checked={packingChecklist.doubleBox}
                    onChange={(e) => setPackingChecklist({...packingChecklist, doubleBox: e.target.checked})}
                    className="w-4.5 h-4.5 rounded border-slate-300 bg-white text-blue-600 accent-blue-600 focus:ring-0 focus:ring-offset-0 cursor-pointer"
                  />
                  <span>Double corrugated cartons for electronics</span>
                </label>

                <label className="flex items-center gap-3 text-xs text-slate-600 cursor-pointer">
                  <input
                    type="checkbox"
                    checked={packingChecklist.heavyItemsBottom}
                    onChange={(e) => setPackingChecklist({...packingChecklist, heavyItemsBottom: e.target.checked})}
                    className="w-4.5 h-4.5 rounded border-slate-300 bg-white text-blue-600 accent-blue-600 focus:ring-0 focus:ring-offset-0 cursor-pointer"
                  />
                  <span>Heavy items arranged at bottom of boxes</span>
                </label>

                <label className="flex items-center gap-3 text-xs text-slate-600 cursor-pointer">
                  <input
                    type="checkbox"
                    checked={packingChecklist.fragileTags}
                    onChange={(e) => setPackingChecklist({...packingChecklist, fragileTags: e.target.checked})}
                    className="w-4.5 h-4.5 rounded border-slate-300 bg-white text-blue-600 accent-blue-600 focus:ring-0 focus:ring-offset-0 cursor-pointer"
                  />
                  <span>"FRAGILE" stickers visible on box exterior</span>
                </label>
              </div>
            </div>
          </div>
        )}

        {/* STEP 6: EXTRA INVENTORY ADD */}
        {currentStep === 6 && (
          <div className="space-y-4 animate-fade-in text-slate-800">
            <div className="bg-slate-50 border border-slate-200 p-4 rounded-3xl space-y-4">
              <h3 className="text-xs font-extrabold text-slate-700 uppercase tracking-widest border-b border-slate-200 pb-2">
                Unlisted Extra Items
              </h3>

              <div className="space-y-2 bg-white p-3 rounded-xl border border-slate-200">
                <span className="text-[10px] text-slate-400 font-extrabold uppercase tracking-widest block">Add Unlisted Goods Form</span>
                
                <div className="flex gap-2">
                  <input
                    type="text"
                    placeholder="e.g. Plastic Stools, Bicycle, Cooler"
                    value={newExtraName}
                    onChange={(e) => setNewExtraName(e.target.value)}
                    className="flex-1 bg-slate-50 border border-slate-300 text-slate-800 rounded-lg px-3 py-2 text-xs outline-none focus:border-blue-500 font-semibold"
                  />
                  <div className="flex items-center bg-slate-50 border border-slate-300 rounded-lg px-1 text-xs">
                    <button 
                      onClick={() => setNewExtraQty(Math.max(1, newExtraQty - 1))}
                      className="px-2 py-1 text-slate-500 hover:text-slate-900 font-bold"
                    >
                      -
                    </button>
                    <span className="px-2 font-bold text-slate-800">{newExtraQty}</span>
                    <button 
                      onClick={() => setNewExtraQty(newExtraQty + 1)}
                      className="px-2 py-1 text-slate-500 hover:text-slate-900 font-bold"
                    >
                      +
                    </button>
                  </div>
                </div>

                <button
                  onClick={handleAddExtraInventory}
                  className="w-full py-2 bg-blue-50 border border-blue-200 text-blue-600 font-bold text-xs rounded-lg flex items-center justify-center gap-1 hover:bg-blue-100 transition-all cursor-pointer"
                >
                  <Plus className="w-3.5 h-3.5" />
                  <span>Log Unlisted Item (+ ₹450 / item)</span>
                </button>
              </div>

              {extraItems.length > 0 ? (
                <div className="space-y-2">
                  <span className="text-[10px] font-extrabold text-slate-500 uppercase tracking-widest block">Logged Extras Schedule</span>
                  <div className="space-y-2">
                    {extraItems.map((item, index) => (
                      <div key={index} className="flex justify-between items-center bg-white p-2.5 rounded-xl border border-slate-200 text-xs">
                        <div className="flex items-center gap-2">
                          <span className="bg-blue-50 text-blue-600 font-extrabold px-2 py-0.5 rounded-md">{item.qty}x</span>
                          <span className="font-bold text-slate-800">{item.name}</span>
                        </div>
                        <div className="flex items-center gap-3">
                          <span className="text-slate-600 font-bold">₹{item.cost}</span>
                          <button 
                            onClick={() => handleRemoveExtraItem(index)}
                            className="text-slate-400 hover:text-rose-600 p-1 cursor-pointer"
                          >
                            <X className="w-4 h-4" />
                          </button>
                        </div>
                      </div>
                    ))}
                    
                    {/* Bill adjustment total */}
                    <div className="bg-blue-50 border border-blue-150 p-3 rounded-xl flex justify-between items-center mt-3 text-xs">
                      <span className="font-bold text-slate-700">Total Adjusted Amount:</span>
                      <span className="font-black text-blue-600 text-sm">
                        + ₹{extraItems.reduce((acc, curr) => acc + curr.cost, 0)}
                      </span>
                    </div>
                  </div>
                </div>
              ) : (
                <div className="bg-white border border-slate-100 p-4 rounded-xl text-center text-xs text-slate-400">
                  No extra items logged. Bill remains as quoted.
                </div>
              )}
            </div>
          </div>
        )}

        {/* STEP 7: DIGITAL SIGNATURE */}
        {currentStep === 7 && (
          <div className="space-y-4 animate-fade-in text-slate-800">
            {/* Customer Digital Signature Card */}
            <div className="bg-slate-50 border border-slate-200 p-4 rounded-3xl space-y-4">
              <h3 className="text-xs font-extrabold text-slate-700 uppercase tracking-widest border-b border-slate-200 pb-2">
                Customer Digital Signature
              </h3>

              <p className="text-xs text-slate-500 leading-relaxed">
                Provide your mobile device to the customer to sign off on the pre-packing inventory lists and declarations.
              </p>

              <div className="relative bg-white rounded-2xl overflow-hidden border border-slate-200 p-1">
                <canvas
                  id="customer-sig-canvas"
                  ref={canvasRef}
                  width={340}
                  height={180}
                  onMouseDown={startDrawing}
                  onMouseMove={draw}
                  onMouseUp={stopDrawing}
                  onMouseLeave={stopDrawing}
                  onTouchStart={startDrawing}
                  onTouchMove={draw}
                  onTouchEnd={stopDrawing}
                  className="w-full bg-slate-50 rounded-xl block touch-none cursor-crosshair"
                />
                
                {/* floating clear button */}
                <button
                  onClick={clearCanvas}
                  className="absolute bottom-3 left-3 bg-white/90 border border-slate-200 text-slate-700 text-[10px] font-bold px-3 py-1.5 rounded-lg hover:bg-slate-100 cursor-pointer"
                >
                  Reset Draw
                </button>

                {signatureSaved && (
                  <div className="absolute inset-0 bg-white/95 backdrop-blur-sm flex flex-col items-center justify-center text-center p-4">
                    <div className="w-10 h-10 bg-emerald-500 rounded-full flex items-center justify-center text-white mb-2">
                      <Check className="w-6 h-6" />
                    </div>
                    <span className="text-xs font-extrabold text-slate-900 uppercase tracking-wider">Customer Signature Saved & Locked</span>
                    <button
                      onClick={() => setSignatureSaved(false)}
                      className="text-[10px] text-blue-600 hover:underline mt-2 cursor-pointer font-bold"
                    >
                      Draw again
                    </button>
                  </div>
                )}
              </div>

              {!signatureSaved && (
                <button
                  onClick={saveSignature}
                  disabled={!hasSigned}
                  className={`w-full py-3 font-extrabold text-xs uppercase tracking-widest rounded-xl transition-all cursor-pointer text-center ${
                    hasSigned 
                      ? 'bg-[#1e8875] hover:bg-[#166c5d] text-white shadow-md shadow-teal-500/10' 
                      : 'bg-slate-100 text-slate-400 border border-slate-200 cursor-not-allowed'
                  }`}
                >
                  💾 Save Customer Signature
                </button>
              )}
            </div>

            {/* Supervisor Digital Signature Card */}
            <div className="bg-slate-50 border border-slate-200 p-4 rounded-3xl space-y-4">
              <h3 className="text-xs font-extrabold text-slate-700 uppercase tracking-widest border-b border-slate-200 pb-2">
                Supervisor Digital Signature
              </h3>

              <p className="text-xs text-slate-500 leading-relaxed">
                Supervisor must sign to certify packing standard checklist, crew guidelines, and logistics readiness.
              </p>

              <div className="relative bg-white rounded-2xl overflow-hidden border border-slate-200 p-1">
                <canvas
                  id="supervisor-sig-canvas"
                  ref={supervisorCanvasRef}
                  width={340}
                  height={180}
                  onMouseDown={startSupervisorDrawing}
                  onMouseMove={drawSupervisor}
                  onMouseUp={stopSupervisorDrawing}
                  onMouseLeave={stopSupervisorDrawing}
                  onTouchStart={startSupervisorDrawing}
                  onTouchMove={drawSupervisor}
                  onTouchEnd={stopSupervisorDrawing}
                  className="w-full bg-slate-50 rounded-xl block touch-none cursor-crosshair"
                />
                
                {/* floating clear button */}
                <button
                  onClick={clearSupervisorCanvas}
                  className="absolute bottom-3 left-3 bg-white/90 border border-slate-200 text-slate-700 text-[10px] font-bold px-3 py-1.5 rounded-lg hover:bg-slate-100 cursor-pointer"
                >
                  Reset Draw
                </button>

                {supervisorSignatureSaved && (
                  <div className="absolute inset-0 bg-white/95 backdrop-blur-sm flex flex-col items-center justify-center text-center p-4">
                    <div className="w-10 h-10 bg-emerald-500 rounded-full flex items-center justify-center text-white mb-2">
                      <Check className="w-6 h-6" />
                    </div>
                    <span className="text-xs font-extrabold text-slate-900 uppercase tracking-wider">Supervisor Signature Saved & Locked</span>
                    <button
                      onClick={() => setSupervisorSignatureSaved(false)}
                      className="text-[10px] text-blue-600 hover:underline mt-2 cursor-pointer font-bold"
                    >
                      Draw again
                    </button>
                  </div>
                )}
              </div>

              {!supervisorSignatureSaved && (
                <button
                  onClick={saveSupervisorSignature}
                  disabled={!supervisorHasSigned}
                  className={`w-full py-3 font-extrabold text-xs uppercase tracking-widest rounded-xl transition-all cursor-pointer text-center ${
                    supervisorHasSigned 
                      ? 'bg-[#1e8875] hover:bg-[#166c5d] text-white shadow-md shadow-teal-500/10' 
                      : 'bg-slate-100 text-slate-400 border border-slate-200 cursor-not-allowed'
                  }`}
                >
                  💾 Save Supervisor Signature
                </button>
              )}
            </div>
          </div>
        )}

        {/* STEP 8: PAYMENT LINK */}
        {currentStep === 8 && (
          <div className="space-y-4 animate-fade-in text-slate-800">
            <div className="bg-slate-50 border border-slate-200 p-5 rounded-3xl text-center space-y-4">
              <div className="space-y-1">
                <span className="text-[10px] text-slate-400 font-extrabold uppercase tracking-widest block">Total Quotation Balance</span>
                <span className="text-2xl font-black text-slate-900 tracking-tight">
                  ₹{18450 + extraItems.reduce((acc, curr) => acc + curr.cost, 0)}
                </span>
                <span className="inline-block bg-blue-50 border border-blue-200 text-blue-600 font-extrabold text-[9px] px-2.5 py-1 rounded-full uppercase mt-1">
                  Includes {extraItems.length} extras
                </span>
              </div>

              {/* UPI QR simulation */}
              <div className="w-40 h-40 bg-white p-3 rounded-2xl mx-auto flex flex-col items-center justify-center shadow-lg border border-slate-200 relative">
                <QrCode className="w-full h-full text-slate-900" />
                <div className="absolute inset-0 bg-slate-500/5 hover:bg-transparent transition-colors rounded-2xl" />
              </div>

              <div className="space-y-2">
                <p className="text-xs text-slate-500 font-medium">
                  Scan this dynamic corporate UPI code to complete immediate checkout.
                </p>
                <div className="flex justify-center gap-2">
                  <button 
                    onClick={() => alert('Secure payment link copied to clipboard!')}
                    className="text-[10px] font-bold text-blue-600 bg-blue-50 border border-blue-200 px-3 py-1.5 rounded-lg hover:underline cursor-pointer"
                  >
                    Copy Payment Link
                  </button>
                </div>
              </div>

              <div className="pt-2 border-t border-slate-200">
                {paymentStatus === 'pending' && (
                  <button
                    onClick={handleVerifyPayment}
                    className="w-full py-3 bg-blue-600 hover:bg-blue-700 text-white font-extrabold text-xs uppercase tracking-widest rounded-xl transition-all cursor-pointer shadow-md shadow-blue-500/10"
                  >
                    🔄 Verify Payment Status
                  </button>
                )}

                {paymentStatus === 'verifying' && (
                  <div className="flex items-center justify-center gap-3 py-3 bg-slate-50 text-slate-700 rounded-xl border border-slate-200">
                    <svg className="animate-spin h-5 w-5 text-blue-600" fill="none" viewBox="0 0 24 24">
                      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
                      <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" />
                    </svg>
                    <span className="text-xs font-bold uppercase tracking-wider">Verifying Gateway...</span>
                  </div>
                )}

                {paymentStatus === 'success' && (
                  <div className="bg-emerald-50 border border-emerald-200 p-3 rounded-xl flex flex-col items-center gap-1 text-emerald-700">
                    <div className="flex items-center gap-2">
                      <Check className="w-4 h-4" />
                      <span className="text-xs font-extrabold uppercase tracking-wider">Payment Verified Successful</span>
                    </div>
                    <span className="text-[9px] text-slate-500">Receipt Ref: PY-9182379-G</span>
                  </div>
                )}
              </div>
            </div>
          </div>
        )}

        {/* STEP 9: WAREHOUSE REACH */}
        {currentStep === 9 && (
          <div className="space-y-4 animate-fade-in text-slate-800">
            <div className="bg-slate-50 border border-slate-200 p-4 rounded-3xl space-y-4">
              <h3 className="text-xs font-extrabold text-slate-700 uppercase tracking-widest border-b border-slate-200 pb-2">
                Transit Warehouse Arrival Check-In
              </h3>

              <div className="space-y-3">
                <div className="space-y-1">
                  <label className="text-[10px] font-extrabold text-slate-500 uppercase tracking-widest block">Select Warehouse Hub</label>
                  <select
                    value={selectedWarehouse}
                    onChange={(e) => setSelectedWarehouse(e.target.value)}
                    className="w-full bg-white border border-slate-300 text-slate-800 rounded-xl px-3 py-2.5 text-xs outline-none focus:border-blue-500 font-semibold cursor-pointer"
                  >
                    <option value="Nelamangala Central Hub">Nelamangala Central Hub (Bangalore)</option>
                    <option value="Peenya Storage Yard">Peenya Storage Yard (North Bangalore)</option>
                    <option value="Thanjavur Regional Logistics Office">Thanjavur Regional Office</option>
                  </select>
                </div>

                {!warehouseArrived ? (
                  <button
                    onClick={handleLogWarehouseArrival}
                    className="w-full py-3 bg-blue-600 hover:bg-blue-700 text-white font-extrabold text-xs uppercase tracking-widest rounded-xl transition-all cursor-pointer shadow-md shadow-blue-500/10"
                  >
                    📍 Log Arrival at Hub
                  </button>
                ) : (
                  <div className="bg-emerald-50 border border-emerald-200 p-3.5 rounded-xl space-y-2">
                    <div className="flex items-center gap-2 text-emerald-700">
                      <Check className="w-4 h-4 animate-bounce" />
                      <span className="text-xs font-extrabold uppercase tracking-wider">Arrival Timestamp Logged</span>
                    </div>
                    <div className="grid grid-cols-2 gap-2 text-[10px] text-slate-500 font-medium">
                      <span>Hub: <strong className="text-slate-800">{selectedWarehouse}</strong></span>
                      <span>Time: <strong className="text-slate-800">{warehouseTime}</strong></span>
                    </div>
                  </div>
                )}
              </div>
            </div>

            <div className="bg-slate-50 border border-slate-200 p-4 rounded-2xl text-xs text-slate-500 leading-relaxed">
              Upon reaching our logistics terminal, log the precise arrival to update client and CRM portal tracking widgets.
            </div>
          </div>
        )}

        {/* STEP 10: DISPATCH DONE */}
        {currentStep === 10 && (
          <div className="space-y-4 animate-fade-in text-slate-800">
            <div className="bg-slate-50 border border-slate-200 p-4 rounded-3xl space-y-4">
              <h3 className="text-xs font-extrabold text-slate-700 uppercase tracking-widest border-b border-slate-200 pb-2">
                Log Dispatch & Lock Container
              </h3>

              <div className="space-y-3.5">
                <div className="space-y-1.5">
                  <label className="text-[10px] font-extrabold text-slate-500 uppercase tracking-widest block">Metal/Electronic Seal Number</label>
                  <input
                    type="text"
                    placeholder="e.g. SL-91823-A"
                    value={sealNo}
                    onChange={(e) => setSealNo(e.target.value.toUpperCase())}
                    className="w-full bg-white border border-slate-300 text-slate-800 rounded-xl px-3 py-2.5 text-xs outline-none focus:border-blue-500 font-extrabold tracking-widest"
                  />
                  <span className="text-[9px] text-slate-400 block leading-tight">Enter the unique serial stamped on the highroad container seal lock.</span>
                </div>

                <div className="pt-2">
                  {!dispatchConfirmed ? (
                    <button
                      onClick={() => {
                        if (!sealNo.trim()) {
                          alert('Please enter container seal number');
                          return;
                        }
                        setDispatchConfirmed(true);
                      }}
                      className="w-full py-3 bg-blue-600 hover:bg-blue-700 text-white font-extrabold text-xs uppercase tracking-widest rounded-xl transition-all cursor-pointer shadow-md"
                    >
                      🚀 Lock Seal & Confirm Dispatch
                    </button>
                  ) : (
                    <div className="bg-emerald-50 border border-emerald-200 p-3.5 rounded-xl text-center space-y-1">
                      <div className="flex items-center justify-center gap-2 text-emerald-700">
                        <Check className="w-4 h-4 font-black" />
                        <span className="text-xs font-extrabold uppercase tracking-wider">Highroad Route Dispatched</span>
                      </div>
                      <span className="text-[10px] text-slate-500 block font-semibold">Container Seal Lock #{sealNo} Registered</span>
                    </div>
                  )}
                </div>
              </div>
            </div>
          </div>
        )}

        {/* STEP 11: REACH DELIVERY POINT */}
        {currentStep === 11 && (
          <div className="space-y-4 animate-fade-in text-slate-800">
            <div className="bg-slate-50 border border-slate-200 p-4 rounded-3xl space-y-4">
              <h3 className="text-xs font-extrabold text-slate-700 uppercase tracking-widest border-b border-slate-200 pb-2">
                Log Arrival at Delivery Point
              </h3>

              <div className="space-y-3">
                {!destArrived ? (
                  <button
                    onClick={handleLogDestArrival}
                    className="w-full py-3 bg-blue-600 hover:bg-blue-700 text-white font-extrabold text-xs uppercase tracking-widest rounded-xl transition-all cursor-pointer shadow-md shadow-blue-500/10"
                  >
                    📍 Log Destination Arrival
                  </button>
                ) : (
                  <div className="bg-emerald-50 border border-emerald-200 p-3.5 rounded-xl space-y-2">
                    <div className="flex items-center gap-2 text-emerald-700">
                      <Check className="w-4 h-4 animate-bounce" />
                      <span className="text-xs font-extrabold uppercase tracking-wider">Arrival Timestamp Logged</span>
                    </div>
                    <div className="grid grid-cols-2 gap-2 text-[10px] text-slate-500 font-medium">
                      <span>Destination: <strong className="text-slate-800">Customer Location</strong></span>
                      <span>Time: <strong className="text-slate-800">{destArrivalTime}</strong></span>
                    </div>
                  </div>
                )}

                <div className="space-y-2 bg-white p-3 rounded-xl border border-slate-200">
                  <span className="text-[10px] font-extrabold text-slate-400 block uppercase">Logistics Handover Verification</span>
                  
                  <label className="flex items-center gap-3 text-xs text-slate-600 cursor-pointer pt-1">
                    <input
                      type="checkbox"
                      checked={unloadedVerified}
                      onChange={(e) => setUnloadedVerified(e.target.checked)}
                      className="w-4.5 h-4.5 rounded border-slate-300 bg-white text-blue-600 accent-blue-600 focus:ring-0 focus:ring-offset-0 cursor-pointer"
                    />
                    <span>Unpacked & checked all scheduled inventory items</span>
                  </label>
                </div>
              </div>
            </div>
            <div className="bg-slate-50 border border-slate-200 p-4 rounded-2xl text-xs text-slate-500 leading-relaxed">
              Log the precise arrival at the customer delivery address and check off items during unloading to verify complete cargo transit.
            </div>
          </div>
        )}

        {/* STEP 12: OTP FOR DELIVERY */}
        {currentStep === 12 && (
          <div className="space-y-4 animate-fade-in text-slate-800">
            <div className="bg-slate-50 border border-slate-200 p-4 rounded-3xl space-y-4">
              <h3 className="text-xs font-extrabold text-slate-700 uppercase tracking-widest border-b border-slate-200 pb-2">
                OTP Verification for Delivery
              </h3>

              <p className="text-xs text-slate-500 leading-relaxed">
                Send secure handover OTP verification PIN code to the customer's registered mobile number (+91 6364354615) to authenticate safe receipt of goods.
              </p>

              <div className="space-y-4 pt-1">
                {!deliveryOtpSent ? (
                  <button
                    onClick={handleSendDeliveryOtp}
                    className="w-full py-3 bg-blue-600 hover:bg-blue-700 text-white font-extrabold text-xs uppercase tracking-widest rounded-xl transition-all cursor-pointer flex items-center justify-center gap-2 shadow-md"
                  >
                    <Smartphone className="w-4 h-4" />
                    <span>Send Handover OTP</span>
                  </button>
                ) : (
                  <div className="space-y-3 animate-fade-in">
                    <div className="bg-blue-50 border border-blue-100 p-3 rounded-xl flex items-center gap-2.5 text-blue-700">
                      <KeyRound className="w-4 h-4 shrink-0" />
                      <span className="text-xs font-semibold">OTP Code sent to recipient phone. Enter 5812.</span>
                    </div>

                    {!deliveryOtpVerified ? (
                      <div className="flex gap-2">
                        <input
                          type="text"
                          maxLength={4}
                          placeholder="Enter 4-digit code"
                          value={deliveryOtpCode}
                          onChange={(e) => setDeliveryOtpCode(e.target.value.replace(/\D/g, ''))}
                          className="flex-1 bg-white border border-slate-300 text-slate-800 text-center text-sm font-extrabold rounded-xl px-3 py-2.5 outline-none focus:border-blue-500 tracking-widest"
                        />
                        <button
                          onClick={handleVerifyDeliveryOtp}
                          className="px-5 bg-blue-600 hover:bg-blue-700 text-white font-extrabold text-xs uppercase tracking-widest rounded-xl cursor-pointer transition-colors"
                        >
                          Verify
                        </button>
                      </div>
                    ) : (
                      <div className="bg-emerald-50 border border-emerald-200 p-3 rounded-xl flex items-center justify-center gap-2 text-emerald-700 font-extrabold text-xs uppercase tracking-widest">
                        <Check className="w-4 h-4 text-emerald-600" />
                        <span>OTP Verified Successfully</span>
                      </div>
                    )}

                    {deliveryOtpError && (
                      <p className="text-[10.5px] text-rose-600 font-bold text-center mt-1">⚠️ {deliveryOtpError}</p>
                    )}

                    {!deliveryOtpVerified && (
                      <button
                        onClick={handleSendDeliveryOtp}
                        className="text-[10px] font-bold text-blue-600 hover:underline block text-center mx-auto cursor-pointer"
                      >
                        Resend OTP Code
                      </button>
                    )}
                  </div>
                )}
              </div>
            </div>
          </div>
        )}

        {/* STEP 13: UNPACKING PHOTO */}
        {currentStep === 13 && (
          <div className="space-y-4 animate-fade-in text-slate-800">
            <div className="bg-slate-50 border border-slate-200 p-4 rounded-3xl space-y-4">
              <div className="flex justify-between items-center border-b border-slate-200 pb-2">
                <h3 className="text-xs font-extrabold text-slate-700 uppercase tracking-widest">
                  Unpacking Goods Gallery ({unpackingPhotos.length})
                </h3>
                <span className="text-[9px] font-bold text-amber-600 uppercase tracking-widest">★ Minimum 1 Required</span>
              </div>

              {unpackingPhotos.length === 0 ? (
                <div className="text-center py-8 text-slate-400 font-medium">No unpacking photos uploaded yet</div>
              ) : (
                <div className="grid grid-cols-3 gap-2">
                  {unpackingPhotos.map((p, i) => (
                    <div key={i} className="relative aspect-square bg-white rounded-xl overflow-hidden group border border-slate-200 shadow-sm">
                      <img src={p} alt="Unpacked Goods" className="w-full h-full object-cover" />
                      <button
                        onClick={() => setUnpackingPhotos(unpackingPhotos.filter((_, idx) => idx !== i))}
                        className="absolute top-1 right-1 w-6 h-6 bg-rose-50 border border-rose-200 text-rose-600 rounded-lg flex items-center justify-center cursor-pointer hover:bg-rose-100 shadow-sm"
                      >
                        <X className="w-3.5 h-3.5" />
                      </button>
                    </div>
                  ))}
                </div>
              )}

              <button
                onClick={handleAddUnpackingPhoto}
                disabled={capturingUnpacking}
                className="w-full py-3 bg-blue-50 hover:bg-blue-100 text-blue-600 border border-blue-200 font-bold text-xs uppercase tracking-widest rounded-xl transition-all flex items-center justify-center gap-2 cursor-pointer"
              >
                <Camera className="w-4 h-4" />
                <span>{capturingUnpacking ? 'Capturing Photo...' : '📸 Simulate Capture Unpacked Goods'}</span>
              </button>
            </div>

            <div className="bg-slate-50 border border-slate-200 p-4 rounded-2xl text-xs text-slate-500 leading-relaxed shadow-xs flex items-start gap-2">
              <Info className="w-4 h-4 text-blue-600 shrink-0 mt-0.5" />
              <p>
                Capture photographs of electronics and major appliances fully unpacked to verify arrival condition without pre-delivery transit marks.
              </p>
            </div>
          </div>
        )}

        {/* STEP 14: DIGITAL DELIVERY SIGNATURE */}
        {currentStep === 14 && (
          <div className="space-y-4 animate-fade-in text-slate-800">
            <div className="bg-slate-50 border border-slate-200 p-4 rounded-3xl space-y-4">
              <h3 className="text-xs font-extrabold text-slate-700 uppercase tracking-widest border-b border-slate-200 pb-2">
                Digital Handover Signature
              </h3>

              <p className="text-xs text-slate-500 leading-relaxed">
                Provide your mobile device to the customer to sign off on the successful receipt and unpacking of all consignment cargo.
              </p>

              <div className="relative bg-white rounded-2xl overflow-hidden border border-slate-200 p-1">
                <canvas
                  id="delivery-sig-canvas"
                  ref={deliveryCanvasRef}
                  width={340}
                  height={180}
                  onMouseDown={startDeliveryDrawing}
                  onMouseMove={drawDelivery}
                  onMouseUp={stopDeliveryDrawing}
                  onMouseLeave={stopDeliveryDrawing}
                  onTouchStart={startDeliveryDrawing}
                  onTouchMove={drawDelivery}
                  onTouchEnd={stopDeliveryDrawing}
                  className="w-full h-[180px] bg-slate-50 border border-slate-100 rounded-xl cursor-crosshair block"
                />
                
                {deliverySignatureSaved && (
                  <div className="absolute inset-0 bg-white/95 flex flex-col items-center justify-center text-emerald-600 animate-fade-in">
                    <div className="w-10 h-10 rounded-full bg-emerald-50 border border-emerald-200 flex items-center justify-center shadow-inner mb-2">
                      <Check className="w-5 h-5 font-black text-emerald-600" />
                    </div>
                    <span className="text-xs font-extrabold text-slate-900 uppercase tracking-wider">Handover Signature Saved</span>
                    <button
                      onClick={() => setDeliverySignatureSaved(false)}
                      className="text-[10px] text-blue-600 hover:underline mt-2 cursor-pointer font-bold"
                    >
                      Draw again
                    </button>
                  </div>
                )}
              </div>

              {!deliverySignatureSaved && (
                <div className="flex gap-2">
                  <button
                    onClick={clearDeliveryCanvas}
                    className="flex-1 py-3 bg-slate-100 text-slate-700 font-extrabold text-xs uppercase tracking-widest rounded-xl transition-all cursor-pointer border border-slate-200 hover:bg-slate-250"
                  >
                    🧹 Clear
                  </button>
                  <button
                    onClick={saveDeliverySignature}
                    disabled={!deliveryHasSigned}
                    className={`flex-1 py-3 font-extrabold text-xs uppercase tracking-widest rounded-xl transition-all cursor-pointer text-center ${
                      deliveryHasSigned 
                        ? 'bg-blue-600 hover:bg-blue-700 text-white shadow-md' 
                        : 'bg-slate-100 text-slate-400 border border-slate-200 cursor-not-allowed'
                    }`}
                  >
                    💾 Save Signature
                  </button>
                </div>
              )}
            </div>
          </div>
        )}

        {/* STEP 15: APPLICATION - RATING */}
        {currentStep === 15 && (
          <div className="space-y-4 animate-fade-in text-slate-800">
            <div className="bg-slate-50 border border-slate-200 p-4 rounded-3xl space-y-4">
              <h3 className="text-xs font-extrabold text-slate-700 uppercase tracking-widest border-b border-slate-200 pb-2">
                Application Rating & Feedback
              </h3>

              <div className="space-y-4">
                <div className="space-y-1.5 text-center">
                  <label className="text-[10px] font-extrabold text-slate-500 uppercase tracking-widest block mb-1">Customer Experience Rating</label>
                  <div className="flex justify-center gap-2">
                    {[1, 2, 3, 4, 5].map((star) => (
                      <button
                        key={star}
                        onClick={() => setAppRating(star)}
                        className="p-1 cursor-pointer transition-transform active:scale-95"
                      >
                        <Star className={`w-8 h-8 ${star <= appRating ? 'text-amber-500 fill-amber-500' : 'text-slate-300'}`} />
                      </button>
                    ))}
                  </div>
                </div>

                <div className="space-y-1">
                  <label className="text-[10px] font-extrabold text-slate-500 uppercase tracking-widest block">Customer Review & Comments</label>
                  <textarea
                    placeholder="Provide details about app user experience or moving team behavior..."
                    value={appRatingComment}
                    onChange={(e) => setAppRatingComment(e.target.value)}
                    rows={3}
                    className="w-full bg-white border border-slate-300 text-slate-800 rounded-xl px-3 py-2 text-xs outline-none focus:border-blue-500 font-medium placeholder-slate-400"
                  />
                </div>
              </div>
            </div>
            <div className="bg-slate-50 border border-slate-200 p-4 rounded-2xl text-xs text-slate-500 leading-relaxed">
              We highly value feedback. Let the customer evaluate the service quality to help our logistics network maintain maximum operational ratings.
            </div>
          </div>
        )}

        {/* STEP 16: TIPS */}
        {currentStep === 16 && (
          <div className="space-y-4 animate-fade-in text-slate-800">
            <div className="bg-slate-50 border border-slate-200 p-4 rounded-3xl space-y-4">
              <h3 className="text-xs font-extrabold text-slate-700 uppercase tracking-widest border-b border-slate-200 pb-2">
                Log Mover Team tips
              </h3>

              <p className="text-xs text-slate-500 leading-relaxed">
                Log voluntary tipping contributions for our heavy-lifting crew to reward stellar packing and delivery service.
              </p>

              <div className="space-y-4">
                <div className="grid grid-cols-5 gap-1.5">
                  {[
                    { value: 'none', label: 'No Tip' },
                    { value: '100', label: '₹100' },
                    { value: '200', label: '₹200' },
                    { value: '500', label: '₹500' },
                    { value: 'custom', label: 'Custom' }
                  ].map((opt) => {
                    const isSel = tipOption === opt.value;
                    return (
                      <button
                        key={opt.value}
                        onClick={() => setTipOption(opt.value as any)}
                        className={`text-[10px] font-bold py-2 rounded-lg border transition-all cursor-pointer text-center ${
                          isSel 
                            ? 'bg-[#e6f4f1] border-[#1e8875] text-[#1e8875] font-black' 
                            : 'bg-white border-slate-200 text-slate-600 hover:bg-slate-50'
                        }`}
                      >
                        {opt.label}
                      </button>
                    );
                  })}
                </div>

                {tipOption === 'custom' && (
                  <div className="space-y-1 animate-fade-in">
                    <label className="text-[10px] font-extrabold text-slate-500 uppercase tracking-widest block">Enter Tip Amount (₹)</label>
                    <input
                      type="text"
                      placeholder="e.g. 350"
                      value={customTip}
                      onChange={(e) => setCustomTip(e.target.value.replace(/\D/g, ''))}
                      className="w-full bg-white border border-slate-300 text-slate-800 rounded-xl px-3 py-2 text-xs outline-none focus:border-[#1e8875] font-extrabold"
                    />
                  </div>
                )}

                <div className="pt-2 border-t border-slate-200">
                  <button
                    onClick={handleCompleteJob}
                    className="w-full py-4 rounded-xl font-extrabold text-xs uppercase tracking-widest transition-all cursor-pointer text-center bg-gradient-to-r from-emerald-600 to-teal-600 text-white hover:opacity-95 shadow-md shadow-emerald-500/10 flex items-center justify-center gap-2"
                  >
                    <span>🎉 Complete Consignment & Handover</span>
                  </button>
                </div>
              </div>
            </div>
          </div>
        )}

      </div>

      {/* 4. FIXED STEPPERS NAVIGATION footer */}
      <div className="bg-white border-t border-slate-200 px-4 py-3 flex gap-3 z-30">
        <button
          onClick={() => setCurrentStep(Math.max(1, currentStep - 1))}
          disabled={currentStep === 1}
          className={`flex-1 flex items-center justify-center gap-1.5 py-3 rounded-xl font-bold text-xs cursor-pointer transition-colors ${
            currentStep === 1 
              ? 'bg-slate-50 text-slate-400 border border-slate-200 cursor-not-allowed' 
              : 'bg-slate-50 hover:bg-slate-100 text-slate-700 border border-slate-200'
          }`}
        >
          <ChevronLeft className="w-4 h-4" />
          <span>Previous</span>
        </button>

        <button
          onClick={() => setCurrentStep(Math.min(totalSteps, currentStep + 1))}
          disabled={currentStep === totalSteps || !canProceed()}
          className={`flex-1 flex items-center justify-center gap-1.5 py-3 rounded-xl font-bold text-xs cursor-pointer transition-colors ${
            currentStep === totalSteps || !canProceed()
              ? 'bg-slate-50 text-slate-400 border border-slate-200 cursor-not-allowed'
              : 'bg-blue-600 hover:bg-blue-700 text-white shadow-md shadow-blue-500/10'
          }`}
        >
          <span>Next Stage</span>
          <ChevronRight className="w-4 h-4" />
        </button>
      </div>

      {/* ========================================== */}
      {/* INTERACTIVE MODALS AND OVERLAYS (STEP 1)  */}
      {/* ========================================== */}

      {/* 1. CUSTOMER CHAT MODAL */}
      {chatOpen && (
        <div className="absolute inset-0 bg-black/60 z-50 flex flex-col justify-end animate-fade-in">
          <div className="bg-white rounded-t-3xl h-[85%] flex flex-col shadow-2xl animate-slide-up">
            {/* Header */}
            <div className="px-4 py-4 border-b border-slate-100 flex items-center justify-between bg-slate-50 rounded-t-3xl">
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 rounded-full bg-teal-50 border border-teal-100 flex items-center justify-center text-[#1e8875] font-extrabold">
                  BN
                </div>
                <div>
                  <h4 className="text-xs font-extrabold text-slate-800">Badri Narayanan</h4>
                  <span className="text-[9px] font-bold text-emerald-600 uppercase tracking-widest">● Customer (Online)</span>
                </div>
              </div>
              <button 
                onClick={() => setChatOpen(false)}
                className="w-8 h-8 rounded-full bg-white border border-slate-200 flex items-center justify-center text-slate-500 hover:text-slate-800 cursor-pointer"
              >
                <X className="w-4 h-4" />
              </button>
            </div>

            {/* Messages body */}
            <div className="flex-1 overflow-y-auto p-4 space-y-3.5 bg-slate-50">
              {chatMessages.map((msg, i) => (
                <div 
                  key={i} 
                  className={`flex flex-col max-w-[80%] ${msg.sender === 'user' ? 'ml-auto items-end' : 'mr-auto items-start'}`}
                >
                  <div className={`p-3 rounded-2xl text-xs leading-relaxed ${
                    msg.sender === 'user' 
                      ? 'bg-[#1e8875] text-white rounded-tr-none shadow-sm' 
                      : 'bg-white text-slate-800 border border-slate-200/80 rounded-tl-none shadow-xs'
                  }`}>
                    {msg.text}
                  </div>
                  <span className="text-[8px] text-slate-400 font-bold mt-1 tracking-wider uppercase">{msg.time}</span>
                </div>
              ))}
            </div>

            {/* Input field */}
            <div className="p-3 border-t border-slate-100 bg-white flex gap-2">
              <input
                type="text"
                placeholder="Type your reply..."
                value={messageText}
                onChange={(e) => setMessageText(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === 'Enter') {
                    if (!messageText.trim()) return;
                    const newMsg = { sender: 'user' as const, text: messageText, time: 'Just now' };
                    setChatMessages(prev => [...prev, newMsg]);
                    setMessageText('');
                    
                    setTimeout(() => {
                      setChatMessages(prev => [...prev, {
                        sender: 'customer',
                        text: 'Perfect, thank you! Please let me know once the driver is near NR colony.',
                        time: 'Just now'
                      }]);
                    }, 1200);
                  }
                }}
                className="flex-1 bg-slate-50 border border-slate-200 rounded-xl px-3 text-xs outline-none focus:border-[#1e8875] text-slate-800"
              />
              <button
                onClick={() => {
                  if (!messageText.trim()) return;
                  const newMsg = { sender: 'user' as const, text: messageText, time: 'Just now' };
                  setChatMessages(prev => [...prev, newMsg]);
                  setMessageText('');
                  
                  setTimeout(() => {
                    setChatMessages(prev => [...prev, {
                      sender: 'customer',
                      text: 'Perfect, thank you! Please let me know once the driver is near NR colony.',
                      time: 'Just now'
                    }]);
                  }, 1200);
                }}
                className="px-4 py-2 bg-[#1e8875] hover:bg-[#166c5d] text-white font-extrabold text-xs uppercase tracking-widest rounded-xl cursor-pointer"
              >
                Send
              </button>
            </div>
          </div>
        </div>
      )}

      {/* 2. IMAGE GALLERY POPUP */}
      {imageGalleryOpen && (
        <div className="absolute inset-0 bg-black/75 z-50 flex items-center justify-center p-4 animate-fade-in">
          <div className="bg-white rounded-3xl w-full max-w-sm overflow-hidden flex flex-col shadow-2xl">
            <div className="px-4 py-3.5 border-b border-slate-150 flex justify-between items-center bg-slate-50">
              <span className="text-xs font-black text-slate-800 uppercase tracking-wider">Customer Consignment Cargo</span>
              <button 
                onClick={() => setImageGalleryOpen(false)}
                className="w-7 h-7 rounded-full bg-white border border-slate-200 flex items-center justify-center text-slate-500 hover:text-slate-800 cursor-pointer"
              >
                <X className="w-3.5 h-3.5" />
              </button>
            </div>
            
            <div className="p-4 grid grid-cols-2 gap-3 max-h-[360px] overflow-y-auto">
              {[
                { title: 'Sofa Set Wrapped', url: 'https://images.unsplash.com/photo-1555041469-a586c61ea9bc?auto=format&fit=crop&w=300&q=80' },
                { title: 'Washing Machine Wrapped', url: 'https://images.unsplash.com/photo-1582730149719-61117da7414b?auto=format&fit=crop&w=300&q=80' },
                { title: 'Boxes Stacked', url: 'https://images.unsplash.com/photo-1532453288672-3a27e9be9efd?auto=format&fit=crop&w=300&q=80' },
                { title: 'Television Boxed', url: 'https://images.unsplash.com/photo-1593305841991-05c297ba4575?auto=format&fit=crop&w=300&q=80' }
              ].map((img, i) => (
                <div key={i} className="space-y-1 group">
                  <div className="aspect-square bg-slate-100 rounded-xl overflow-hidden border border-slate-200">
                    <img src={img.url} alt={img.title} className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300" referrerPolicy="no-referrer" />
                  </div>
                  <span className="text-[9px] font-bold text-slate-500 block text-center truncate">{img.title}</span>
                </div>
              ))}
            </div>
            <div className="p-3 bg-slate-50 text-center text-[10px] text-slate-400 font-semibold border-t border-slate-100">
              Pre-transit inventory verification snaps (4)
            </div>
          </div>
        </div>
      )}

      {/* 3. APPROVE OR REQUEST CHANGES MODAL */}
      {approveRequestOpen && (
        <div className="absolute inset-0 bg-black/60 z-50 flex items-center justify-center p-4 animate-fade-in">
          <div className="bg-white rounded-3xl w-full max-w-sm overflow-hidden shadow-2xl p-5 space-y-4">
            <div className="flex justify-between items-start">
              <div className="space-y-0.5">
                <h4 className="text-sm font-black text-slate-800">Inventory Verification Action</h4>
                <p className="text-[10px] text-slate-400 font-bold uppercase tracking-wide">Select Cargo Approval Status</p>
              </div>
              <button 
                onClick={() => setApproveRequestOpen(false)}
                className="w-7 h-7 rounded-full bg-slate-50 border border-slate-200 flex items-center justify-center text-slate-500 hover:text-slate-800 cursor-pointer"
              >
                <X className="w-3.5 h-3.5" />
              </button>
            </div>

            <div className="space-y-3.5 py-2">
              <button
                onClick={() => {
                  setExtraItemsRequested(false);
                  setApproveRequestOpen(false);
                  alert('🎉 Pre-Transit Inventory Approved! You can proceed to starting the vehicle.');
                }}
                className="w-full p-4 bg-teal-50 hover:bg-teal-100/80 border border-teal-200 rounded-2xl flex items-center gap-3.5 text-left transition-colors cursor-pointer"
              >
                <div className="w-9 h-9 rounded-full bg-white flex items-center justify-center text-[#1e8875] shrink-0 border border-teal-100 shadow-sm">
                  <Check className="w-5 h-5" />
                </div>
                <div>
                  <span className="text-xs font-extrabold text-slate-800 block">Approve All Listed Cargo</span>
                  <p className="text-[10px] text-slate-500 mt-0.5">All items perfectly match physical counts.</p>
                </div>
              </button>

              <button
                onClick={() => {
                  setExtraItemsRequested(true);
                  setApproveRequestOpen(false);
                  alert('⚠️ Request Sent! Flagged additional cardboard packing materials for supervisor revision.');
                }}
                className="w-full p-4 bg-amber-50 hover:bg-amber-100/80 border border-amber-200 rounded-2xl flex items-center gap-3.5 text-left transition-colors cursor-pointer"
              >
                <div className="w-9 h-9 rounded-full bg-white flex items-center justify-center text-amber-600 shrink-0 border border-amber-100 shadow-sm">
                  <Plus className="w-5 h-5" />
                </div>
                <div>
                  <span className="text-xs font-extrabold text-slate-800 block">Request Extra Items Addition</span>
                  <p className="text-[10px] text-slate-500 mt-0.5">Report extra luggage/unscheduled boxes found.</p>
                </div>
              </button>
            </div>

            {extraItemsRequested && (
              <div className="bg-amber-50 border border-amber-200 px-3 py-2 rounded-xl text-[10px] text-amber-700 font-bold text-center animate-fade-in">
                ⚠️ Revision Requested: Unscheduled cardboard boxes flagged.
              </div>
            )}
          </div>
        </div>
      )}

      {/* 4. CALLING SUPPORT OVERLAY */}
      {callingSupport && (
        <div className="absolute inset-0 bg-[#1e8875] z-50 flex flex-col justify-between p-8 text-white text-center animate-fade-in">
          <div className="pt-12 space-y-2">
            <span className="text-[10px] font-black tracking-widest uppercase text-teal-100 block">Secure Dial Network</span>
            <h4 className="text-xl font-extrabold">NoBroker Support Line</h4>
            <span className="text-xs font-bold text-teal-100 animate-pulse block">Calling +91 9000000000...</span>
          </div>

          <div className="space-y-6">
            <div className="w-24 h-24 rounded-full bg-white/10 border border-white/20 flex items-center justify-center mx-auto animate-pulse">
              <Phone className="w-10 h-10 text-white" />
            </div>
            <p className="text-xs text-teal-50 max-w-xs mx-auto leading-relaxed">
              Connecting you safely via outbound supervisor line. Please wait for the logistics desk to pickup.
            </p>
          </div>

          <button
            onClick={() => setCallingSupport(false)}
            className="w-full py-4 bg-rose-600 hover:bg-rose-700 text-white font-extrabold text-xs uppercase tracking-widest rounded-2xl cursor-pointer transition-colors shadow-lg"
          >
            End Call
          </button>
        </div>
      )}

    </div>
  );
};

export default JobDetailScreen;
