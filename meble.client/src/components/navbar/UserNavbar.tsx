import  { useContext } from 'react';
import { Link } from 'react-router-dom';
import AuthContext from '../../services/AuthContext'; // Importujemy AuthContext

const UserNavbar = () => {
    const { isLoggedIn } = useContext(AuthContext); // Używamy kontekstu autoryzacji

    if (!isLoggedIn) {
        return null; // Nie wyświetlaj Navbar, jeśli użytkownik nie jest zalogowany
    }

    return (
        <nav className='flex justify-center w-full px-10 items-center'>
            <ul className='flex w-80 flex-col'>
                <li className='transition-transform duration-500 hover:scale-110 px-4 my-3 py-2 flex justify-center items-center rounded-xl text-white bg-[#a68b7c] hover:bg-[#8b6e60]'><Link to="/manage/orders">Zamówienia</Link></li>
                <li className='transition-transform duration-500 hover:scale-110 px-4 my-3 py-2 flex justify-center items-center rounded-xl text-white bg-[#a68b7c] hover:bg-[#8b6e60]'><Link to="/manage/edit">Edytuj Dane</Link></li>
                <li className='transition-transform duration-500 hover:scale-110 px-4 my-3 py-2 flex justify-center items-center rounded-xl text-white bg-[#a68b7c] hover:bg-[#8b6e60]'><Link to="/manage/editUser">Zarządzanie użytkownikiem</Link></li>
                <li className='transition-transform duration-500 hover:scale-110 px-4 my-3 py-2 flex justify-center items-center rounded-xl text-white bg-[#a68b7c] hover:bg-[#8b6e60]'><Link to="/manage/delete">Usuń Konto</Link></li>
            </ul>
        </nav>
    );

};

export default UserNavbar;
