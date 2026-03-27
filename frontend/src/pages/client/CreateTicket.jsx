import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import ClientSidebar from '../../components/ClientSidebar';
import { createTicket, getReferentiels } from '../../api/client';

export default function CreateTicket() {
  const [refs, setRefs]     = useState({ categories: [], priorites: [] });
  const [form, setForm]     = useState({ titre: '', descr: '', id_Pri: '', cod_Cat: '' });
  const [error, setError]   = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    getReferentiels().then(r => setRefs(r.data));
  }, []);

  const set = (k) => (e) => setForm({ ...form, [k]: e.target.value });

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(''); setLoading(true);
    try {
      const payload = {
        titre: form.titre,
        descr: form.descr,
        id_Pri: parseInt(form.id_Pri),
        cod_Cat: parseInt(form.cod_Cat),
      };
      const res = await createTicket(payload);
      navigate(`/tickets/${res.data.num_Tic}`);
    } catch (err) {
      setError(err.response?.data?.message || 'Erreur lors de la création du ticket.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="layout">
      <ClientSidebar />
      <main className="main-content">
        <div className="topbar">
          <h1>Nouveau ticket</h1>
        </div>

        <div className="card" style={{ maxWidth: 680 }}>
          <form onSubmit={handleSubmit}>
            <div className="form-group">
              <label>Titre du problème *</label>
              <input type="text" placeholder="Ex : Mon ordinateur ne démarre plus"
                value={form.titre} onChange={set('titre')} required />
            </div>

            <div className="form-group">
              <label>Description détaillée *</label>
              <textarea placeholder="Décrivez le problème en détail..."
                value={form.descr} onChange={set('descr')} required />
            </div>

            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '0 16px' }}>
              <div className="form-group">
                <label>Catégorie *</label>
                <select value={form.cod_Cat} onChange={set('cod_Cat')} required>
                  <option value="">-- Sélectionner --</option>
                  {refs.categories.map(c => (
                    <option key={c.cod_Cat} value={c.cod_Cat}>
                      {c.libelle} ({c.equipe})
                    </option>
                  ))}
                </select>
              </div>

              <div className="form-group">
                <label>Priorité *</label>
                <select value={form.id_Pri} onChange={set('id_Pri')} required>
                  <option value="">-- Sélectionner --</option>
                  {refs.priorites.map(p => (
                    <option key={p.id_Pri} value={p.id_Pri}>{p.libelle}</option>
                  ))}
                </select>
              </div>
            </div>

            {error && <p className="error-msg">{error}</p>}

            <div style={{ display: 'flex', gap: 12, marginTop: 8 }}>
              <button type="button" className="btn btn-outline"
                onClick={() => navigate('/tickets')}>
                Annuler
              </button>
              <button type="submit" className="btn btn-primary" disabled={loading}>
                {loading ? 'Envoi...' : '🚀 Soumettre le ticket'}
              </button>
            </div>
          </form>
        </div>
      </main>
    </div>
  );
}
