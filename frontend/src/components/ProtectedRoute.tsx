import { Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

interface Props {
  children: React.ReactNode;
  adminOnly?: boolean;
}

const ProtectedRoute: React.FC<Props> = ({ children, adminOnly = false }) => {
  const { token, isAdmin } = useAuth();

  if (!token) return <Navigate to="/login" replace />;
  if (adminOnly && !isAdmin) return <Navigate to="/dashboard" replace />;

  return <>{children}</>;
};

export default ProtectedRoute;
