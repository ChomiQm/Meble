import { Navigate } from 'react-router-dom';
import { useAuth } from '../../services/AuthContext';

interface AdminRouteProps {
    children: React.ReactNode;
}

const AdminRoute: React.FC<AdminRouteProps> = ({ children }) => {
    const { roles } = useAuth();
    const isAdmin = roles && roles.includes('Admin');

    return isAdmin ? children : <Navigate to="/" />;
};

export default AdminRoute;
