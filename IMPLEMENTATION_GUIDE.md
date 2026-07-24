# Unamora-SAAS Implementation Roadmap

## 🎉 COMPLETED PHASE 1: Foundation Setup

### Database & Backend
✅ **Database Migration**: Switched from SQLite to SQL Server (SSMS)
- Connection string: `Server=.;Database=UnamoraDB;Integrated Security=true;TrustServerCertificate=true;`
- File: `backend/src/Unamora.Api/appsettings.Development.json`

✅ **Admin User Seeding**: Automatically created on app startup
- **Username**: NyikoAdmin
- **Password**: Nyikomasinge1
- **Email**: admin@unamora.com
- **Role**: Admin

✅ **Role-Based Infrastructure**:
- Roles seeded: Admin, SuperAdmin, Client, Tradesperson
- Demo users created automatically:
  - Client: client@unamora.com / Password123!
  - Plumber: johndoe@plumbing.co.za / Password123!
  - Electrician: sparky@elec.co.za / Password123!

### Frontend Infrastructure
✅ **React Router v6**: Full routing implementation
- Browser-based routing (no hash-based navigation)
- Protected routes with role-based access control
- Automatic redirects for unauthorized access

✅ **Authentication System**:
- Context-based state management (`AuthContext.tsx`)
- `useAuth()` hook for easy access in components
- Persistent login (localStorage)
- Automatic token management

✅ **HTTP Client** (`api.ts`):
- Axios instance with request/response interceptors
- Request: Automatically adds authorization token
- Response: Handles 401 errors, redirects to login
- All backend endpoints mapped and ready

---

## 🚀 PHASE 2: Pages & Components (IN PROGRESS)

### ✅ Completed Pages
| Page | File | Status | Features |
|------|------|--------|----------|
| Landing | `src/pages/Landing.tsx` | ✅ Complete | Hero, features, CTA with search |
| Login | `src/pages/auth/Login.tsx` | ✅ Complete | Email/password, demo credentials display |
| Register | `src/pages/auth/Register.tsx` | ✅ Complete | 2-step: Role selection, then details |
| Profile Setup | `src/pages/auth/ProfileSetup.tsx` | ✅ Complete | Client & Tradesperson-specific fields |
| Dashboard | `src/pages/Dashboard.tsx` | ✅ Complete | Role-based metrics & actions |
| Search | `src/pages/Search.tsx` | ✅ Complete | Query results with pro cards |
| Layout | `src/components/Layout.tsx` | ✅ Complete | Header, footer, navigation |

### 🚧 Stub Pages (Need Implementation)
- Chat/ChatList & ChatWindow
- Payments/PaymentPage
- Reviews/ReviewDisplay
- Admin/AdminDashboard

---

## 📋 PHASE 3: Component Library & Styling (NEXT)

### Required Reusable Components
Create these in `src/components/`:

```typescript
// Form Components
- Button.tsx (primary, secondary, small variants)
- Input.tsx (text, email, password, tel variants)
- Textarea.tsx
- Select.tsx
- Checkbox.tsx
- RadioButton.tsx

// Display Components
- Card.tsx
- Avatar.tsx
- Badge.tsx
- Rating.tsx (star component)
- LoadingSpinner.tsx
- ErrorMessage.tsx
- Toast/Alert.tsx

// Layout Components
- Modal.tsx
- Tabs.tsx
- Accordion.tsx
- Breadcrumb.tsx
```

### CSS Updates Needed
- Auth page styles (login, register, profile setup)
- New dashboard layouts
- Form styling with validation states
- Responsive mobile layouts
- Color scheme consistency

---

## 🎯 PHASE 4: Feature Pages

### 1. **Profile Pages** (LinkedIn-style)
- [ ] Profile Display Page (`src/pages/Profile/ViewProfile.tsx`)
  - Photo, headline, bio
  - Experience, education, portfolio
  - Reviews, skills, ratings
  - Action buttons (book, message, save)

- [ ] Profile Edit Page (`src/pages/Profile/EditProfile.tsx`)
  - File upload for photo
  - All profile fields
  - Add/remove skills & trades
  - Work history for tradespersons

### 2. **Booking Flow**
- [ ] Job Request Form (`src/pages/Bookings/CreateJobRequest.tsx`)
  - Service category selection
  - Job description
  - Budget preference
  - Location & date/time
  
- [ ] View Quotes (`src/pages/Bookings/Quotes.tsx`)
  - List of quotes
  - Compare professionals
  - Accept/reject quote
  
- [ ] Booking Management (`src/pages/Bookings/MyBookings.tsx`)
  - Active & past bookings
  - Job timeline
  - Tracking status

### 3. **Enhanced Chat** (Complete Implementation)
- [ ] Conversation List
  - Real-time message count
  - Last message preview
  - Typing indicator
  
- [ ] Chat Window
  - Message history
  - Image/file attachments
  - Read receipts
  - Message reactions

### 4. **Payment & Billing**
- [ ] Payment Methods
  - Add/remove cards
  - Set default
  
- [ ] Invoice History
  - Download invoices
  - Payment status
  - Receipt tracking
  
- [ ] Subscription Management
  - Current plan
  - Upgrade/downgrade
  - Billing cycle

### 5. **Reviews & Ratings**
- [ ] Write Review
  - Star rating
  - Comment
  - Photo upload
  
- [ ] View Reviews
  - All reviews list
  - Filter by rating
  - Respond to reviews

### 6. **Admin Dashboard** (Enhanced)
- [ ] Verification Queue
  - Document list
  - Approval workflow
  - Audit trail
  
- [ ] Dispute Management
  - Dispute list
  - Detail view
  - Resolution tools
  
- [ ] User Management
  - User list
  - Roles & permissions
  - Suspend/activate users
  
- [ ] Reports & Analytics
  - Revenue charts
  - User growth
  - Service metrics

---

## 🔧 Implementation Instructions

### Step 1: Install Dependencies & Run
```bash
cd Unamora_SAAS
npm install
npm run dev
```

### Step 2: Start Backend
```bash
# Ensure SQL Server is running
# Open Visual Studio or run:
cd backend/src/Unamora.Api
dotnet run
```

### Step 3: Test Authentication
1. Visit: http://localhost:5173
2. Click "Create your account"
3. Choose role (Client or Tradesperson)
4. Fill in profile details
5. You'll be redirected to dashboard

### Step 4: Test Admin Panel
1. Sign out
2. Go to login
3. Use: NyikoAdmin / Nyikomasinge1
4. You'll see admin dashboard with queue, disputes, risk cases

---

## 🎨 Design System Reference

### Colors
```css
--color-primary: #13231d;      /* Deep forest green */
--color-accent: #ed684a;        /* Terracotta orange */
--color-secondary: #52635b;     /* Muted sage */
--color-bg: #f7f7f2;            /* Off-white cream */
--color-border: #e0e0e0;
--color-success: #4caf50;
--color-error: #f44336;
--color-warning: #ff9800;
```

### Typography
- **Headings**: Bold sans-serif, loose letter-spacing
- **Body**: 1.08rem, 1.65 line-height
- **Labels**: 800-weight, all-caps

### Components
- **Buttons**: Rounded (999px), dark bg, white text
- **Cards**: Padding 32px, radius 16px, subtle shadows
- **Inputs**: White bg, border, radius 8px
- **Grids**: Responsive CSS Grid with auto-columns

---

## 📝 Code Examples

### Using useAuth Hook
```typescript
import { useAuth } from '../context/AuthContext';

function MyComponent() {
  const { user, login, logout, isAuthenticated } = useAuth();
  
  return (
    <div>
      {isAuthenticated && <p>Welcome, {user?.firstName}</p>}
    </div>
  );
}
```

### Making API Calls
```typescript
import { api } from '../services/api';

async function loadProfile() {
  try {
    const response = await api.getTradespersonProfile(userId);
    setProfile(response.data);
  } catch (error) {
    console.error('Failed to load profile');
  }
}
```

### Protected Route Example
```typescript
<Route 
  path="/admin" 
  element={<ProtectedRoute requiredRole="Admin"><AdminPanel /></ProtectedRoute>} 
/>
```

---

## 🐛 Common Issues & Solutions

### "Cannot find module" errors
- Run `npm install` in Unamora_SAAS folder
- Restart dev server

### 401 Unauthorized errors
- Check token in localStorage
- Ensure backend is running
- Verify connection string in appsettings

### Database not syncing
- Check: Server=.; Database=UnamoraDB
- Ensure SQL Server is running
- Run migrations if needed

### Styles not loading
- Check App.css is imported
- Ensure CSS files are in `src/` folder
- Restart dev server after CSS changes

---

## 📞 API Endpoints Reference

All endpoints are in `src/services/api.ts` as methods:

```typescript
// Auth
api.login(credentials)
api.register(data)
api.logout()

// Profiles
api.getClientProfile(userId)
api.updateClientProfile(userId, data)
api.getTradespersonProfile(userId)
api.updateTradespersonProfile(userId, data)

// Search & Booking
api.searchTradespersons(filters)
api.createJobRequest(data)
api.getBookings(userId)
api.getQuotes(userId)

// Chat
api.getChatConversations(userId)
api.getMessages(conversationId)
api.sendMessage(data)

// Payments & Reviews
api.getPayments(userId)
api.createPayment(data)
api.getReviews(userId)
api.createReview(data)

// Admin
api.getAdminDashboard()
api.getVerificationQueue()
api.getDisputes()
```

---

## ✅ Next Steps Checklist

- [ ] Run `npm install` in Unamora_SAAS
- [ ] Verify database connection
- [ ] Start backend (`dotnet run`)
- [ ] Start frontend (`npm run dev`)
- [ ] Test login with demo credentials
- [ ] Fix any component export issues
- [ ] Update CSS for new pages
- [ ] Build component library
- [ ] Create profile pages
- [ ] Implement booking flow
- [ ] Complete admin features

---

**Created**: 2026-07-24
**Version**: Phase 1 Complete
**Status**: Ready for Phase 2 implementation
