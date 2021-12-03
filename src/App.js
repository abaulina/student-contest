import React from 'react';
import { BrowserRouter as Router, Route, Routes, Link } from 'react-router-dom';
import { Toaster } from 'react-hot-toast';
import NotFound from './components/errorPages/notFound';
import BadRequest from './components/errorPages/badRequest';
import logo from './assets/logo192.png';
import { Login } from './components/auth/login.component/login.component';
import { Main } from './components/main.component';
import { SignUp } from './components/auth/signup.component/signup.component';
import './App.css';
import '../node_modules/bootstrap/dist/css/bootstrap.css';

function App() {
	return (
		<Router>
			<Toaster position='top-right' />
			<div className='App container-fluid'>
				<NavBar />
				<div className='auth-wrapper'>
					<Routes>
						<Route exact path='/' element={<Main />} />
						<Route path='/login' element={<Login />} />
						<Route path='/signup' element={<SignUp />} />
						<Route path='/error' element={<BadRequest />} />
						<Route path='*' element={<NotFound />} />
					</Routes>
				</div>
			</div>
		</Router>
	);
}

const NavBar = () => (
	<nav className='navbar navbar-expand-lg navbar-light'>
		<div className='container'>
			<Link className='navbar-brand' to={'/'}>
				<img
					src={logo}
					alt=''
					height='150'
					className='d-inline-block align-text-bottom'
				/>
			</Link>
			<div className='d-flex justify-content-end' id='navbarTogglerDemo02'>
				<div className='navbar-nav ml-auto'>
					<button className='btn'>
						<Link className='nav-link' to={'/login'}>
							Log In
						</Link>
					</button>
					<button className='button info'>
						<Link className='nav-link' to={'/signup'}>
							Sign up
						</Link>
					</button>
				</div>
			</div>
		</div>
	</nav>
);

export default App;
