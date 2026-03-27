import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';

// Client pages
import LoginClient   from './pages/client/LoginClient';
import RegisterClient from './pages/client/RegisterClient';
import ClientTickets from './pages/client/ClientTickets';
import CreateTicket  from './pages/client/CreateTicket';
import TicketDetail  from './pages/client/TicketDetail';

// Agent pages
import LoginAgent       from './pages/agent/LoginAgent';
import AgentDashboard   from './pages/agent/AgentDashboard';
import AgentTicketDetail from './pages/agent/AgentTicketDetail';

import 'bootstrap/dist/css/bootstrap.min.css';
import './index.css';

function PrivateRoute({ children, requiredRole }) {
  const { user } = useAuth();
  if (!user) return <Navigate to="/" replace />;
  if (requiredRole && user.role !== requiredRole && user.role !== 'Superviseur')
    return <Navigate to="/" replace />;
  return children;
}

function RootRedirect() {
  const { user } = useAuth();
  if (!user) return <Navigate to="/login" replace />;
  if (user.role === 'Client') return <Navigate to="/tickets" replace />;
  return <Navigate to="/agent/dashboard" replace />;
}

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/"       element={<RootRedirect />} />
          <Route path="/login"  element={<LoginClient />} />
          <Route path="/register" element={<RegisterClient />} />

          {/* Client routes */}
          <Route path="/tickets" element={
            <PrivateRoute requiredRole="Client"><ClientTickets /></PrivateRoute>
          } />
          <Route path="/tickets/new" element={
            <PrivateRoute requiredRole="Client"><CreateTicket /></PrivateRoute>
          } />
          <Route path="/tickets/:id" element={
            <PrivateRoute requiredRole="Client"><TicketDetail /></PrivateRoute>
          } />

          {/* Agent routes */}
          <Route path="/agent/login"    element={<LoginAgent />} />
          <Route path="/agent/dashboard" element={
            <PrivateRoute requiredRole="Agent"><AgentDashboard /></PrivateRoute>
          } />
          <Route path="/agent/tickets/:id" element={
            <PrivateRoute requiredRole="Agent"><AgentTicketDetail /></PrivateRoute>
          } />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}
