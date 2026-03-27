import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { registerClient } from '../../api/client';
import { useAuth } from '../../context/AuthContext';

export default function RegisterClient() {
  const [form, setForm] = useState({ nom: '', prenom: '', email: '', tel: '', login: '', mdp: '' });
  const [error,  setError]  = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate  = useNavigate();

  const set = (k) => (e) => setForm({ ...form, [k]: e.target.value });

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(''); setLoading(true);
    try {
      const res = await registerClient(form);
      login(res.data.token);
      navigate('/tickets');
    } catch (err) {
      setError(err.response?.data?.message || 'Erreur lors de l\'inscription.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-wrapper">
      <div className="auth-card">
        <h1>Créer un compte</h1>
        <p className="subtitle">Rejoignez le portail Helpdesk CHK</p>

        <form onSubmit={handleSubmit}>
          <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '0 16px' }}>
            <div className="form-group">
              <label>Nom</label>
              <input type="text" placeholder="Dupont" value={form.nom} onChange={set('nom')} required />
            </div>
            <div className="form-group">
              <label>Prénom</label>
              <input type="text" placeholder="Jean" value={form.prenom} onChange={set('prenom')} required />
            </div>
          </div>
          <div className="form-group">
            <label>Email</label>
            <input type="email" placeholder="jean.dupont@chk.com" value={form.email} onChange={set('email')} required />
          </div>
          <div className="form-group">
            <label>Téléphone (optionnel)</label>
            <input type="tel" placeholder="+33 6 00 00 00 00" value={form.tel} onChange={set('tel')} />
          </div>
          <div className="form-group">
            <label>Login</label>
            <input type="text" placeholder="jdupont" value={form.login} onChange={set('login')} required />
          </div>
          <div className="form-group">
            <label>Mot de passe</label>
            <input type="password" placeholder="••••••••" value={form.mdp} onChange={set('mdp')} required minLength={6} />
          </div>

          {error && <p className="error-msg">{error}</p>}

          <button type="submit" className="btn btn-primary" style={{ width: '100%', marginTop: 8 }}
            disabled={loading}>
            {loading ? 'Création...' : 'Créer mon compte'}
          </button>
        </form>

        <p style={{ textAlign: 'center', marginTop: 20, fontSize: '.88rem', color: 'var(--clr-muted)' }}>
          Déjà un compte ?{' '}
          <Link to="/login" style={{ color: 'var(--clr-primary)', fontWeight: 600 }}>
            Se connecter
          </Link>
        </p>
      </div>
    </div>
  );
}
