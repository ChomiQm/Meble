import React from 'react';
import { Outlet, Navigate } from 'react-router-dom';
import { useAuth } from '../../services/AuthContext';

const PrivateRoute: React.FC = () => {
    const { isLoggedIn } = useAuth();

    if (!isLoggedIn) {
        return <Navigate to="/login" />;
    }

    return <Outlet />;
};

export default PrivateRoute;
