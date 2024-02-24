import React from 'react';
import { Outlet, Navigate } from 'react-router-dom';
import { useAuth } from '../../services/AuthContext'; // Zaimportuj hook useAuth

const PrivateRoute: React.FC = () => {
    const { isLoggedIn } = useAuth(); // Pobierz stan logowania z AuthContext

    if (!isLoggedIn) {
        console.log("Użytkownik nie jest zalogowany. Przekierowanie do logowania.");
        return <Navigate to="/login" />;
    }

    console.log("Użytkownik jest zalogowany. Kontynuacja przekierowania.");
    return <Outlet />;
};

export default PrivateRoute;
