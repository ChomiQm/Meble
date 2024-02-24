import { Link } from 'react-router-dom';
import { useAuth } from '../../services/AuthContext';
import LogoutButton from '../button/LogoutButton';
import useLogout from '../../hooks/useLogout';
import { useState } from 'react';

function Navbar() {
    const { isLoggedIn, hasData, roles } = useAuth();
    const handleLogout = useLogout();
    const isAdmin = roles.includes('Admin');
    const [isOpen, setIsOpen] = useState(false);

    const toggleMenu = () => {
        setIsOpen(!isOpen);
    };
    

    return (
        <nav>
            <div className='hidden md:block pb-8'>
                <ul className='flex containerCustom py-10 items-center justify-between gap-2 md:gap-10'>
                    <li className='transition hover:scale-105 text-white text-center'><Link to="/" className='border-2 border-white rounded-full px-4 py-2 hover:bg-white hover:text-black bg-transparent'>Strona główna</Link></li>
                    {!isLoggedIn ? (
                        <div className='flex justify-center items-center gap-10'>
                            <li className='transition hover:scale-105 border-2 border-white rounded-full px-4 py-2 text-white text-center hover:bg-white hover:text-black bg-transparent'><Link to="/login">Login</Link></li>
                            <li className='transition hover:scale-105 border-2 border-white rounded-full px-4 py-2 text-white text-center hover:bg-white hover:text-black bg-transparent'><Link to="/register">Rejestracja</Link></li>
                        </div>
                    ) : (
                        <div className='flex text-white gap-2 md:gap-12 items-center justify-center'>
                            {isLoggedIn && hasData && <li className='transform transition-transform hover:scale-105'><Link to="/manage" className='border-2 border-white rounded-full px-4 py-2 hover:bg-white hover:text-black bg-transparent'>Zarządzaj</Link></li>}
                            {isLoggedIn && hasData && <li className='transform transition-transform hover:scale-105'><Link to="/shop" className='border-2 border-white rounded-full px-4 py-2 hover:bg-white hover:text-black bg-transparent'>Sklep</Link></li>}
                                {isLoggedIn && hasData && <li className='transform transition-transform hover:scale-105'><Link to="/cart" className='border-2 border-white rounded-full px-4 py-2 hover:bg-white hover:text-black bg-transparent'>Koszyk</Link></li>}
                                {isLoggedIn && isAdmin && hasData && <li className='transform transition-transform hover:scale-105'><Link to="/adminPanel" className='border-2 border-white rounded-full px-4 py-2 hover:bg-white hover:text-black bg-transparent'>Admin Panel</Link></li>}
                            <li><LogoutButton onLogout={handleLogout} /></li>
                        </div>
                    )}
                </ul>
            </div>
            <div className='md:hidden'>

            <div id="menu" className={`text-center overlay fixed top-0 z-50 left-0 h-full w-64 bg-gray-800 text-white overflow-hidden transition-transform transform ${isOpen ? 'blurred' : ''} ${isOpen ? '' : '-translate-x-full'}`}>

                <div className='w-full h-full flex'> 
                        <li className="w-full list-none">
                        <Link onClick={toggleMenu} className='block w-full px-4 py-2 text-white font-semibold hover:bg-gray-200 hover:text-gray-900 transition-colors duration-300 border-t border-b border-gray-300' to="/">
                            Strona Główna
                        </Link>

                        {!isLoggedIn && (
                            <>
                                <Link onClick={toggleMenu} className='block w-full px-4 py-2 text-white font-semibold hover:bg-gray-200 hover:text-gray-900 transition-colors duration-300 border-t border-b border-gray-300' to="/login">
                                    Login
                                </Link>
                                <Link onClick={toggleMenu} className='block w-full px-4 py-2 text-white font-semibold hover:bg-gray-200 hover:text-gray-900 transition-colors duration-300 border-t border-b border-gray-300' to="/register">
                                    Rejestracja
                                </Link>
                            </>
                        )}

                        {isLoggedIn && hasData && 
                            <Link onClick={toggleMenu} className='block w-full px-4 py-2 text-white font-semibold hover:bg-gray-200 hover:text-gray-900 transition-colors duration-300 border-t border-b border-gray-300' to="/manage">
                                Zarządzaj
                            </Link>
                        }
                        {isLoggedIn && hasData && 
                            <Link onClick={toggleMenu} className='block w-full px-4 py-2 text-white font-semibold hover:bg-gray-200 hover:text-gray-900 transition-colors duration-300 border-t border-b border-gray-300' to="/shop">
                                Sklep
                            </Link>
                        }
                        {isLoggedIn && hasData && 
                            <Link onClick={toggleMenu} className='block w-full px-4 py-2 text-white font-semibold hover:bg-gray-200 hover:text-gray-900 transition-colors duration-300 border-t border-b border-gray-300' to="/cart">
                                Koszyk
                            </Link>
                        }
                        {isLoggedIn && hasData && isAdmin &&
                            <Link onClick={toggleMenu} className='block w-full px-4 py-2 text-white font-semibold hover:bg-gray-200 hover:text-gray-900 transition-colors duration-300 border-t border-b border-gray-300' to="/adminPanel">
                                AdminPanel
                            </Link>
                            }
                        {isLoggedIn &&
                            <Link onClick={() => { handleLogout(); toggleMenu(); }} className='block w-full px-4 py-2 text-white font-semibold hover:bg-gray-200 hover:text-gray-900 transition-colors duration-300 border-t border-b border-gray-300' to="/">
                            Wyloguj
                            </Link>
                        }
                        <Link to="" onClick={toggleMenu} className='block w-full px-4 py-2 text-white font-semibold hover:bg-gray-200 hover:text-gray-900 transition-colors duration-300 border-t border-b border-gray-300'>
                            Zamknij menu
                        </Link>   
                    </li>      
                </div>
            </div>
                <button id="toggleMenu"  onClick={toggleMenu} className="text-white z-50 focus:outline-none focus:text-gray-800 bg-transparent">
                    <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M4 6h16M4 12h16m-7 6h7"></path>
                    </svg>
                </button>
            </div>
        </nav>
        
    );
}

export default Navbar;
