import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import ClientSidebar from '../../components/ClientSidebar';
import { getTickets } from '../../api/client';

const statusBadgeClass = (s) => {
  if (s === 'Initialisé') return 'badge badge-init';
  if (s === 'En cours')   return 'badge badge-enc';
  if (s === 'Résolu')     return 'badge badge-res';
  if (s === 'Fermé')      return 'badge badge-ferm';
  return 'badge';
};

const prioBadgeClass = (p) => {
  if (!p) return 'badge';
  return `badge badge-${p.toLowerCase()}`;
};

export default function ClientTickets() {
  const [tickets, setTickets] = useState([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    getTickets().then(r => { setTickets(r.data); setLoading(false); })
               .catch(() => setLoading(false));
  }, []);

  return (
    <div className="layout">
      <ClientSidebar />
      <main className="main-content">
        <div className="topbar">
          <h1>Mes tickets</h1>
          <button className="btn btn-primary" onClick={() => navigate('/tickets/new')}>
            + Nouveau ticket
          </button>
        </div>

        <div className="card">
          {loading ? (
            <p style={{ color: 'var(--clr-muted)' }}>Chargement...</p>
          ) : tickets.length === 0 ? (
            <div style={{ textAlign: 'center', padding: '40px 0' }}>
              <p style={{ fontSize: '2.5rem' }}>🎉</p>
              <p style={{ color: 'var(--clr-muted)', marginTop: 8 }}>
                Aucun ticket pour le moment. Tout va bien !
              </p>
              <button className="btn btn-primary" style={{ marginTop: 16 }}
                onClick={() => navigate('/tickets/new')}>
                Créer mon premier ticket
              </button>
            </div>
          ) : (
            <div className="table-wrap">
              <table>
                <thead>
                  <tr>
                    <th>#</th>
                    <th>Titre</th>
                    <th>Catégorie</th>
                    <th>Priorité</th>
                    <th>Statut</th>
                    <th>Date</th>
                  </tr>
                </thead>
                <tbody>
                  {tickets.map(t => (
                    <tr key={t.num_Tic} onClick={() => navigate(`/tickets/${t.num_Tic}`)}>
                      <td style={{ fontWeight: 700, color: 'var(--clr-primary)' }}>#{t.num_Tic}</td>
                      <td style={{ fontWeight: 500 }}>{t.titre}</td>
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
          )}
        </div>
      </main>
    </div>
  );
}
