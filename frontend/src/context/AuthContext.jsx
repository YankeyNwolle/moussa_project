import { createContext, useContext, useState, useEffect } from 'react';
import { jwtDecode } from 'jwt-decode';

const AuthContext = createContext(null);

function getDecodedRole(decoded) {
  return (
    decoded?.role ||
    decoded?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
    decoded?.['roles']
  );
}

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const decoded = jwtDecode(token);
        if (decoded.exp * 1000 > Date.now()) {
          const role = getDecodedRole(decoded);
          setUser({
            id:         decoded.userId,
            login:      decoded.unique_name,
            role,
            nomComplet: decoded.nomComplet,
            token,
          });
        } else {
          localStorage.removeItem('token');
        }
      } catch {
        localStorage.removeItem('token');
      }
    }
  }, []);

  const login = (token) => {
    localStorage.setItem('token', token);
    const decoded = jwtDecode(token);
    const role = getDecodedRole(decoded);
    setUser({
      id:         decoded.userId,
      login:      decoded.unique_name,
      role,
      nomComplet: decoded.nomComplet,
      token,
    });
  };

  const logout = () => {
    localStorage.removeItem('token');
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => useContext(AuthContext);
