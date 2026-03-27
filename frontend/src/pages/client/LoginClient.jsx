import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { loginClient } from '../../api/client';
import { useAuth } from '../../context/AuthContext';

export default function LoginClient() {
  const [form, setForm] = useState({ login: '', mdp: '' });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate  = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(''); setLoading(true);
    try {
      const res = await loginClient(form);
      login(res.data.token);
      navigate('/tickets');
    } catch (err) {
      setError(err.response?.data?.message || 'Erreur de connexion.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-wrapper">
      <div className="auth-card">
        <h1>Connexion Client</h1>
        <p className="subtitle">Accédez à vos tickets de support</p>

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>Login</label>
            <input type="text" placeholder="Votre login"
              value={form.login}
              onChange={e => setForm({ ...form, login: e.target.value })} required />
          </div>
          <div className="form-group">
            <label>Mot de passe</label>
            <input type="password" placeholder="••••••••"
              value={form.mdp}
              onChange={e => setForm({ ...form, mdp: e.target.value })} required />
          </div>

          {error && <p className="error-msg">{error}</p>}

          <button type="submit" className="btn btn-primary" style={{ width: '100%', marginTop: 8 }}
            disabled={loading}>
            {loading ? 'Connexion...' : 'Se connecter'}
          </button>
        </form>

        <p style={{ textAlign: 'center', marginTop: 20, fontSize: '.88rem', color: 'var(--clr-muted)' }}>
          Pas encore de compte ?{' '}
          <Link to="/register" style={{ color: 'var(--clr-primary)', fontWeight: 600 }}>
            S'inscrire
          </Link>
        </p>
        <p style={{ textAlign: 'center', marginTop: 8, fontSize: '.88rem', color: 'var(--clr-muted)' }}>
          Vous êtes un agent ?{' '}
          <Link to="/agent/login" style={{ color: 'var(--clr-accent)', fontWeight: 600 }}>
            Accès agent
          </Link>
        </p>
      </div>
    </div>
  );
}
