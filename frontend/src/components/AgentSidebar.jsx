import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const DashIcon = () => (
  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
    <rect x="3" y="3" width="7" height="7"/><rect x="14" y="3" width="7" height="7"/>
    <rect x="14" y="14" width="7" height="7"/><rect x="3" y="14" width="7" height="7"/>
  </svg>
);

const TicketIcon = () => (
  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
    <path d="M15 5v2M15 11v2M15 17v2M5 5h14a2 2 0 012 2v3a2 2 0 000 4v3a2 2 0 01-2 2H5a2 2 0 01-2-2v-3a2 2 0 000-4V7a2 2 0 012-2z"/>
  </svg>
);

const LogoutIcon = () => (
  <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
    <path d="M9 21H5a2 2 0 01-2-2V5a2 2 0 012-2h4M16 17l5-5-5-5M21 12H9"/>
  </svg>
);

export default function AgentSidebar() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => { logout(); navigate('/agent/login'); };

  return (
    <aside className="sidebar">
      <div className="sidebar-logo">
        <h2>🛠 Helpdesk <span>CHK</span></h2>
        <p style={{ fontSize: '.78rem', color: 'rgba(255,255,255,.6)', marginTop: 4 }}>
          Agent : {user?.nomComplet}
        </p>
      </div>
      <nav className="sidebar-nav">
        <NavLink to="/agent/dashboard" className={({isActive}) => `sidebar-link ${isActive ? 'active' : ''}`}>
          <DashIcon /> Tableau de bord
        </NavLink>
        <NavLink to="/agent/dashboard" end={false} className="sidebar-link" style={{ opacity: .6, pointerEvents: 'none' }}>
          <TicketIcon /> Mes tickets
        </NavLink>
      </nav>
      <div className="sidebar-footer">
        <button className="sidebar-link" onClick={handleLogout} style={{ background: 'none', border: 'none', cursor: 'pointer', width: '100%' }}>
          <LogoutIcon /> Déconnexion
        </button>
      </div>
    </aside>
  );
}
