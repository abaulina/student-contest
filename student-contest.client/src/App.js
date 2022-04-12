import React from 'react';
import {
	unstable_HistoryRouter as HistoryRouter,
	Route,
	Routes,
	Link
} from 'react-router-dom';
import cn from 'classnames';
import { Toaster } from 'react-hot-toast';
import { WrappedLogin } from './auth/login/login';
import Main from './main/mainPage';
import NotFound from './errors/errorPages/notFound';
import DefaultErrorPage from './errors/errorPages/default';
import { WrappedSignup } from './auth/signup/signup';
import PrivatePage from './privatePage';
import useAuth, { AuthProvider } from './auth/useAuth';
import { WrappedUserAccount } from './user/userAccount';
import history from './utilities/history';
import logo from './assets/logo192.png';
import './App.css';
import '../node_modules/bootstrap/dist/css/bootstrap.css';

function App() {
	return (
		<AuthProvider>
			<HistoryRouter history={history}>
				<Toaster position='top-right' />
				<div className='App container-fluid'>
					<NavBar />
					<div className='auth-wrapper'>
						<Routes>
							<Route exact path='/' element={<Main />} />
							<Route path='/login' element={<WrappedLogin />} />
							<Route path='/signup' element={<WrappedSignup />} />
							<Route path='/user' element={<WrappedUserAccount />} />
							<Route path='/test' element={<PrivatePage />} />
							<Route path='/error' element={<DefaultErrorPage />} />
							<Route path='*' element={<NotFound />} />
						</Routes>
					</div>
				</div>
			</HistoryRouter>
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
