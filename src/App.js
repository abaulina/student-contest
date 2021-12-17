import React, { useState } from 'react';
import { BrowserRouter as Router, Route, Routes, Link } from 'react-router-dom';
import cn from 'classnames';
import Cookies from 'js-cookie';
import PropTypes from 'prop-types';
import { Toaster } from 'react-hot-toast';
import Login from './auth/login/login';
import Main from './main/mainPage';
import NotFound from './error404/notFound';
//import PrivateRoute from './PrivateRoute';
import SignUp from './auth/signup/signup';
import UserAccount from './user/userAccount';
import logo from './assets/logo192.png';
import './App.css';
import '../node_modules/bootstrap/dist/css/bootstrap.css';

function App() {
	const [isUserLoggedIn, setUserLoggedIn] = useState(
		Cookies.get('userEmail') ? true : false
	);

	return (
		<Router>
			<Toaster position='top-right' />
			<div className='App container-fluid'>
				<NavBar
					isUserLoggedIn={isUserLoggedIn}
					setUserLoggedIn={setUserLoggedIn}
				/>
				<div className='auth-wrapper'>
					<Routes>
						<Route
							exact
							path='/'
							element={
								<Main
									isUserLoggedIn={isUserLoggedIn}
									setUserLoggedIn={setUserLoggedIn}
								/>
							}
						/>
						<Route
							path='/login'
							element={<Login setUserLoggedIn={setUserLoggedIn} />}
						/>
						<Route path='/signup' element={<SignUp />} />
						<Route path='/user' element={<UserAccount />} />
						<Route path='*' element={<NotFound />} />
					</Routes>
				</div>
			</div>
		</Router>
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

const NavBarLogoutButton = ({ setUserLoggedIn }) => {
	const LogOut = () => {
		setUserLoggedIn(false);
		Cookies.remove('userEmail');
	};

	return (
		<button className='button info' onClick={LogOut}>
			Log out
		</button>
	);
};

const NavBar = (props) => {
	const loginButtonClassName = cn({
		btn: true,
		invisible: props.isUserLoggedIn
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
						{props.isUserLoggedIn ? (
							<NavBarLogoutButton setUserLoggedIn={props.setUserLoggedIn} />
						) : (
							<NavBarSignUpButton />
						)}
					</div>
				</div>
			</div>
		</nav>
	);
};

NavBar.propTypes = {
	isUserLoggedIn: PropTypes.bool.isRequired,
	setUserLoggedIn: PropTypes.func.isRequired
};

NavBarLogoutButton.propTypes = {
	setUserLoggedIn: PropTypes.func.isRequired
};

export default App;
