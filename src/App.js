import React from 'react';
import { BrowserRouter as Router, Route, Routes, Link } from 'react-router-dom';
import cn from 'classnames';
import Cookies from 'js-cookie';
import { Toaster } from 'react-hot-toast';
import Login from './auth/login/login';
import Main from './main/mainPage';
import NotFound from './error404/notFound';
import SignUp from './auth/signup/signup';
import PrivatePage from './privatePage';
import useAuth, { AuthProvider } from './auth/useAuth';
import { WrapperUserAccount } from './user/userAccount';
import logo from './assets/logo192.png';
import './App.css';
import '../node_modules/bootstrap/dist/css/bootstrap.css';

function App() {
	return (
		<AuthProvider>
			<Router>
				<Toaster position='top-right' />
				<div className='App container-fluid'>
					<NavBar />
					<div className='auth-wrapper'>
						<Routes>
							<Route exact path='/' element={<Main />} />
							<Route path='/login' element={<Login />} />
							<Route path='/signup' element={<SignUp />} />
							<Route path='/user' element={<WrapperUserAccount />} />
							<Route path='/test' element={<PrivatePage />} />
							<Route path='*' element={<NotFound />} />
						</Routes>
					</div>
				</div>
			</Router>
		</AuthProvider>
	);
}

const NavBarLogo = () => (
	<Link className='navbar-brand' to={'/'}>
		<img
			src={logo}
			alt=''
			height='150'
			className='d-inline-block align-text-bottom'
		/>
	</Link>
);

const NavBarSignUpButton = () => (
	<button className='button info'>
		<Link className='nav-link' to={'/signup'}>
			Sign up
		</Link>
	</button>
);

const NavBarLogoutButton = () => {
	const auth = useAuth();
	const handleLogOut = () => {
		auth.logout();
		Cookies.remove('userEmail');
	};

	return (
		<button className='button info' onClick={handleLogOut}>
			Log out
		</button>
	);
};

const NavBar = () => {
	const isAuthenticated = useAuth().isAuthenticated;
	const loginButtonClassName = cn({
		btn: true,
		invisible: isAuthenticated
	});

	return (
		<nav className='navbar navbar-expand-lg navbar-light'>
			<div className='container'>
				<NavBarLogo />
				<div className='d-flex justify-content-end'>
					<div className='navbar-nav'>
						<button className={loginButtonClassName}>
							<Link className='nav-link' to={'/login'}>
								Log In
							</Link>
						</button>
						{isAuthenticated ? <NavBarLogoutButton /> : <NavBarSignUpButton />}
					</div>
				</div>
			</div>
		</nav>
	);
};

export default App;
