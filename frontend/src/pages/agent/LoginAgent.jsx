import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { loginAgent } from '../../api/client';
import { useAuth } from '../../context/AuthContext';

export default function LoginAgent() {
  const [form, setForm]     = useState({ login: '', mdp: '' });
  const [error, setError]   = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate  = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(''); setLoading(true);
    try {
      const res = await loginAgent(form);
      login(res.data.token);
      navigate('/agent/dashboard');
    } catch (err) {
      setError(err.response?.data?.message || 'Identifiants incorrects.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-wrapper">
      <div className="auth-card">
        <h1>🛠 Espace Agent</h1>
        <p className="subtitle">Helpdesk GROUPE CHK — Accès réservé aux agents</p>

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>Login agent</label>
            <input type="text" placeholder="Votre login"
              value={form.login} onChange={e => setForm({ ...form, login: e.target.value })} required />
          </div>
          <div className="form-group">
            <label>Mot de passe</label>
            <input type="password" placeholder="••••••••"
              value={form.mdp} onChange={e => setForm({ ...form, mdp: e.target.value })} required />
          </div>

          {error && <p className="error-msg">{error}</p>}

          <button type="submit" className="btn btn-primary" style={{ width: '100%', marginTop: 8 }}
            disabled={loading}>
            {loading ? 'Connexion...' : 'Connexion'}
          </button>
        </form>

        <p style={{ textAlign: 'center', marginTop: 20, fontSize: '.88rem', color: 'var(--clr-muted)' }}>
          Vous êtes un client ?{' '}
          <Link to="/login" style={{ color: 'var(--clr-primary)', fontWeight: 600 }}>
            Portail client
          </Link>
        </p>
      </div>
    </div>
  );
}
