import { useEffect, useRef, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import AgentSidebar from '../../components/AgentSidebar';
import { getTicket, getMessages, sendMessage, getHistory, updateStatus } from '../../api/client';

const STATUTS = [
  { id: 1, label: 'Initialisé' },
  { id: 2, label: 'En cours' },
  { id: 3, label: 'Résolu' },
  { id: 4, label: 'Fermé' },
];

const statusBadgeClass = (s) => {
  if (s === 'Initialisé') return 'badge badge-init';
  if (s === 'En cours')   return 'badge badge-enc';
  if (s === 'Résolu')     return 'badge badge-res';
  if (s === 'Fermé')      return 'badge badge-ferm';
  return 'badge';
};

export default function AgentTicketDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [ticket,   setTicket]   = useState(null);
  const [messages, setMessages] = useState([]);
  const [history,  setHistory]  = useState([]);
  const [newMsg,   setNewMsg]   = useState('');
  const [file,     setFile]     = useState(null);
  const [tab,      setTab]      = useState('messages');
  const [sending,  setSending]  = useState(false);
  const [updating, setUpdating] = useState(false);
  const chatEndRef = useRef(null);

  const load = async () => {
    const [t, m, h] = await Promise.all([getTicket(id), getMessages(id), getHistory(id)]);
    setTicket(t.data);
    setMessages(m.data);
    setHistory(h.data);
  };

  useEffect(() => { load(); }, [id]);
  useEffect(() => { chatEndRef.current?.scrollIntoView({ behavior: 'smooth' }); }, [messages]);

  const handleSend = async (e) => {
    e.preventDefault();
    if (!newMsg.trim() && !file) return;
    setSending(true);
    const fd = new FormData();
    fd.append('cont', newMsg);
    if (file) fd.append('attachment', file);
    try {
      await sendMessage(id, fd);
      setNewMsg(''); setFile(null);
      await load();
    } finally { setSending(false); }
  };

  const handleStatusChange = async (newStatutId) => {
    setUpdating(true);
    try {
      await updateStatus(id, { id_Sta_Apres: parseInt(newStatutId) });
      await load();
    } finally { setUpdating(false); }
  };

  if (!ticket) return <div className="layout"><AgentSidebar /><main className="main-content"><p>Chargement...</p></main></div>;

  return (
    <div className="layout">
      <AgentSidebar />
      <main className="main-content">
        {/* Header */}
        <div className="topbar">
          <div>
            <button className="btn btn-outline" style={{ marginBottom: 12 }}
              onClick={() => navigate('/agent/dashboard')}>← Retour</button>
            <h1>Ticket #{ticket.num_Tic}</h1>
          </div>
          <div style={{ display: 'flex', gap: 12, alignItems: 'center', flexWrap: 'wrap' }}>
            <span className={statusBadgeClass(ticket.statutLibelle)} style={{ fontSize: '.95rem', padding: '8px 16px' }}>
              {ticket.statutLibelle}
            </span>
            <div style={{ display: 'flex', gap: 8 }}>
              {STATUTS.filter(s => s.label !== ticket.statutLibelle).map(s => (
                <button key={s.id} className="btn btn-outline"
                  style={{ fontSize: '.82rem', padding: '6px 12px' }}
                  disabled={updating}
                  onClick={() => handleStatusChange(s.id)}>
                  → {s.label}
                </button>
              ))}
            </div>
          </div>
        </div>

        {/* Ticket info */}
        <div className="card" style={{ marginBottom: 20 }}>
          <h2 style={{ fontWeight: 700, marginBottom: 8 }}>{ticket.titre}</h2>
          <p style={{ color: 'var(--clr-muted)', marginBottom: 16, lineHeight: 1.6 }}>{ticket.descr}</p>
          <div style={{ display: 'flex', gap: 24, flexWrap: 'wrap', fontSize: '.85rem', color: 'var(--clr-muted)' }}>
            <span>👤 Client : <strong>{ticket.clientNom}</strong></span>
            <span>📂 <strong>{ticket.categorieLibelle}</strong></span>
            <span>🔥 <strong>{ticket.prioriteLibelle}</strong></span>
            <span>📅 <strong>{new Date(ticket.datecre).toLocaleString('fr-FR')}</strong></span>
          </div>
        </div>

        {/* Tabs */}
        <div style={{ display: 'flex', gap: 8, marginBottom: 16 }}>
          {['messages', 'history'].map(t => (
            <button key={t} className={`btn ${tab === t ? 'btn-primary' : 'btn-outline'}`}
              onClick={() => setTab(t)}>
              {t === 'messages' ? '💬 Messages' : '🕒 Historique'}
            </button>
          ))}
        </div>

        {tab === 'messages' && (
          <div className="card">
            <div className="chat-container">
              {messages.length === 0
                ? <p style={{ color: 'var(--clr-muted)', textAlign: 'center' }}>Aucun message.</p>
                : messages.map(m => (
                  <div key={m.cod_Mes} className={`chat-bubble ${m.auteurRole === 'Client' ? 'agent' : 'client'}`}>
                    <strong style={{ display: 'block', marginBottom: 4, fontSize: '.78rem', opacity: .8 }}>
                      {m.auteurNom} ({m.auteurRole})
                    </strong>
                    {m.cont}
                    {m.image && (
                      <div style={{ marginTop: 8 }}>
                        <a href={`http://localhost:5001${m.image}`} target="_blank" rel="noreferrer"
                          style={{ color: 'inherit', textDecoration: 'underline', fontSize: '.82rem' }}>
                          📎 Pièce jointe
                        </a>
                      </div>
                    )}
                    <div className="bubble-meta">{new Date(m.date_Mes).toLocaleString('fr-FR')}</div>
                  </div>
                ))
              }
              <div ref={chatEndRef} />
            </div>
            <form onSubmit={handleSend} style={{ marginTop: 16, display: 'flex', gap: 8, flexWrap: 'wrap' }}>
              <input type="text" placeholder="Réponse à l'utilisateur..."
                value={newMsg} onChange={e => setNewMsg(e.target.value)}
                style={{ flex: 1, minWidth: 200 }} />
              <input type="file" onChange={e => setFile(e.target.files[0])} style={{ width: 'auto', padding: '10px 0' }} />
              <button type="submit" className="btn btn-primary" disabled={sending}>
                {sending ? '...' : 'Envoyer'}
              </button>
            </form>
          </div>
        )}

        {tab === 'history' && (
          <div className="card">
            <div className="timeline">
              {history.map((h, i) => (
                <div key={h.id_Cha} className="timeline-item">
                  <div className="timeline-dot">{i + 1}</div>
                  <div className="timeline-body">
                    <p>
                      {h.statutAvant
                        ? <><strong>{h.statutAvant}</strong> → <strong>{h.statutApres}</strong></>
                        : <>Ticket créé → <strong>{h.statutApres}</strong></>}
                    </p>
                    <small>{h.agentNom} · {new Date(h.date_Change).toLocaleString('fr-FR')}</small>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}
      </main>
    </div>
  );
}
