import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import AgentSidebar from '../../components/AgentSidebar';
import { getAgentDashboard, getAgentTickets } from '../../api/client';

const statusBadgeClass = (s) => {
  if (s === 'Initialisé') return 'badge badge-init';
  if (s === 'En cours')   return 'badge badge-enc';
  if (s === 'Résolu')     return 'badge badge-res';
  if (s === 'Fermé')      return 'badge badge-ferm';
  return 'badge';
};

const prioBadgeClass = (p) => `badge badge-${(p || '').toLowerCase()}`;

const STATUTS = ['', 'Initialisé', 'En cours', 'Résolu', 'Fermé'];

export default function AgentDashboard() {
  const navigate  = useNavigate();
  const [stats,   setStats]   = useState(null);
  const [tickets, setTickets] = useState([]);
  const [total,   setTotal]   = useState(0);
  const [filters, setFilters] = useState({ statut: '', prioriteId: '', page: 1 });
  const [loading, setLoading] = useState(true);

  const loadStats = () => getAgentDashboard().then(r => setStats(r.data));
  const loadTickets = (f) =>
    getAgentTickets({ statut: f.statut || undefined, prioriteId: f.prioriteId || undefined, page: f.page })
      .then(r => { setTickets(r.data.items); setTotal(r.data.total); setLoading(false); });

  useEffect(() => {
    loadStats();
    loadTickets(filters);
  }, []);

  const applyFilters = (newF) => {
    const f = { ...filters, ...newF, page: 1 };
    setFilters(f); setLoading(true);
    loadTickets(f);
  };

  const statColors = ['var(--clr-primary)', '#f59e0b', '#22c55e', '#94a3b8'];

  return (
    <div className="layout">
      <AgentSidebar />
      <main className="main-content">
        <div className="topbar">
          <h1>Tableau de bord</h1>
        </div>

        {/* Stats cards */}
        {stats && (
          <div className="card-grid">
            {Object.entries(stats.parStatut).map(([label, count], i) => (
              <div className="stat-card" key={label}>
                <div className="stat-value" style={{ background: `none`, WebkitTextFillColor: statColors[i] || 'var(--clr-primary)', color: statColors[i] }}>
                  {count}
                </div>
                <div className="stat-label">{label}</div>
              </div>
            ))}
            <div className="stat-card">
              <div className="stat-value">{stats.totalTickets}</div>
              <div className="stat-label">Total assignés</div>
            </div>
          </div>
        )}

        {/* Filters */}
        <div className="card">
          <div className="filters-bar">
            <div className="form-group">
              <label>Statut</label>
              <select value={filters.statut} onChange={e => applyFilters({ statut: e.target.value })}>
                {STATUTS.map(s => <option key={s} value={s}>{s || 'Tous'}</option>)}
              </select>
            </div>
            <div className="form-group">
              <label>Priorité</label>
              <select value={filters.prioriteId} onChange={e => applyFilters({ prioriteId: e.target.value })}>
                <option value="">Toutes</option>
                <option value="1">Faible</option>
                <option value="2">Moyenne</option>
                <option value="3">Haute</option>
                <option value="4">Critique</option>
              </select>
            </div>
          </div>

          {loading ? <p style={{ color: 'var(--clr-muted)' }}>Chargement...</p> : (
            <>
              <div className="table-wrap">
                <table>
                  <thead>
                    <tr>
                      <th>#</th>
                      <th>Titre</th>
                      <th>Client</th>
                      <th>Catégorie</th>
                      <th>Priorité</th>
                      <th>Statut</th>
                      <th>Date</th>
                    </tr>
                  </thead>
                  <tbody>
                    {tickets.length === 0 ? (
                      <tr><td colSpan={7} style={{ textAlign: 'center', color: 'var(--clr-muted)', padding: 24 }}>
                        Aucun ticket correspondant.
                      </td></tr>
                    ) : tickets.map(t => (
                      <tr key={t.num_Tic} onClick={() => navigate(`/agent/tickets/${t.num_Tic}`)}>
                        <td style={{ fontWeight: 700, color: 'var(--clr-primary)' }}>#{t.num_Tic}</td>
                        <td style={{ fontWeight: 500 }}>{t.titre}</td>
                        <td style={{ color: 'var(--clr-muted)', fontSize: '.85rem' }}>{t.clientNom}</td>
                        <td style={{ color: 'var(--clr-muted)', fontSize: '.85rem' }}>{t.categorieLibelle}</td>
                        <td><span className={prioBadgeClass(t.prioriteLibelle)}>{t.prioriteLibelle}</span></td>
                        <td><span className={statusBadgeClass(t.statutLibelle)}>{t.statutLibelle}</span></td>
                        <td style={{ color: 'var(--clr-muted)', fontSize: '.85rem' }}>
                          {new Date(t.datecre).toLocaleDateString('fr-FR')}
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
              {/* Pagination */}
              <div style={{ marginTop: 16, display: 'flex', gap: 8, justifyContent: 'flex-end', alignItems: 'center' }}>
                <span style={{ color: 'var(--clr-muted)', fontSize: '.85rem' }}>{total} ticket(s)</span>
                {filters.page > 1 && (
                  <button className="btn btn-outline" onClick={() => {
                    const f = { ...filters, page: filters.page - 1 };
                    setFilters(f); loadTickets(f);
                  }}>←</button>
                )}
                {tickets.length === 20 && (
                  <button className="btn btn-outline" onClick={() => {
                    const f = { ...filters, page: filters.page + 1 };
                    setFilters(f); loadTickets(f);
                  }}>→</button>
                )}
              </div>
            </>
          )}
        </div>
      </main>
    </div>
  );
}
