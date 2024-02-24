import { Route, Routes } from 'react-router-dom';
import '../src/layouts/main.css';
import Login from '../src/pages/Login';
import Register from '../src/pages/Register';
import PrivateRoute from '../src/components/routing/PrivateRoute';
import AdminRoute from '../src/components/routing/AdminRoute';
import AddInfo from '../src/pages/AddInfo';
import Edit from '../src/pages/Edit';
import Navbar from '../src/components/navbar/Navbar';
import { AuthProvider } from './services/AuthContext';
import Manage from '../src/pages/Manage';
import Delete from './pages/Delete';
import EditUser from './pages/EditUser';
import Shop from './pages/Shop';
import Orders from './pages/Order';
import Cart from './pages/Cart';
import MainPage from './pages/MainPage';
import AdminPanel from './pages/AdminPanel'


function App() {
    return (
        <AuthProvider>
            <div>
                <Navbar />
                <Routes>
                    <Route path="/login" element={<Login />} />
                    <Route path="/register" element={<Register />} />
                    <Route element={<PrivateRoute />}>
                        <Route path="/addInfo" element={<AddInfo />} />
                        <Route path="/manage" element={<Manage />} />
                        {<Route path="/manage/edit" element={<Edit />} />}
                        {<Route path="/manage/editUser" element={<EditUser />} />}
                        {<Route path="/manage/delete" element={<Delete />} />}
                        {<Route path="/manage/orders" element={<Orders />} />}
                        {<Route path="/shop" element={<Shop />} />}
                        {<Route path="/cart" element={<Cart />} />}
                    </Route>
                    <Route path="/adminPanel" element={ <AdminRoute> <AdminPanel /> </AdminRoute> } />
                    <Route path="/" element={<MainPage />} />

                </Routes>
            </div>
        </AuthProvider>
    );
}

export default App;
