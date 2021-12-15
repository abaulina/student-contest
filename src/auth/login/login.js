import React, { useEffect, useRef, useState } from 'react';
import Cookies from 'js-cookie';
import { Link } from 'react-router-dom';
import PropTypes from 'prop-types';
import toast from 'react-hot-toast';
import EmailInput from '../emailInput';
import PasswordInput from '../passwordInput';
import './login.css';

const ForgotPasswordLink = () => (
	<p className='forgot-password text-right'>
		<Link to=''>Forgot password?</Link>
	</p>
);

const NoAccountLink = () => (
	<p className='no-account text-right'>
		<Link to='/signup'>No account?</Link>
	</p>
);

function Login(props) {
	const localStorage = window.localStorage;
	const [email, setEmail] = useState('');
	const [password, setPassword] = useState('');
	const [isPasswordShown, setPasswordShown] = useState(false);
	const [isCapsWarningShown, setCapsWarningShown] = useState(false);
	const [error, setError] = useState('');
	const submitButton = useRef();
	const passwordInput = useRef();

	const togglePassword = () => {
		setPasswordShown((isPasswordShown) => !isPasswordShown);
	};

	const capsLockWarning = (e) => {
		if (e.getModifierState('CapsLock') && !isCapsWarningShown) {
			toast.error('Caps lock is on', { icon: '❗' });
			setCapsWarningShown((isCapsWarningShown) => !isCapsWarningShown);
		}
	};

	const onPasswordInput = (e) => {
		setPassword(e.target.value);
	};

	const onEmailInput = (e) => {
		setEmail(e.target.value);
	};

	useEffect(() => {
		if (email) setError(null);
	}, [email]);

	useEffect(() => {
		if (password) setError(null);
	}, [password]);

	const onEnterPress = (e) => {
		if (e.charCode === 13) {
			switch (e.target.id) {
				case 'floatingEmail':
					passwordInput.current.focus();
					break;
				case 'floatingPassword':
					submitButton.current.click();
					break;
				default:
					break;
			}
		}
	};

	const validateInput = () => {
		if (validateEmail() && validatePassword()) {
			doLogin();
		}
	};

	const doLogin = () => {
		setCapsWarningShown((isCapsWarningShown) => !isCapsWarningShown);
		setError(null);
		Cookies.set('userEmail', email, { expires: 30 });
		props.setUserLoggedIn(true);
	};

	const validateEmail = () => {
		const users = JSON.parse(localStorage.getItem('Users')) || [];
		if (!users.some((user) => user.email === email)) {
			setError('Invalid email or password');
			return false;
		}
		return true;
	};

	const validatePassword = () => {
		const users = JSON.parse(localStorage.getItem('Users')) || [];
		if (
			!users.some((user) => user.email === email && user.password === password)
		) {
			setError('Invalid email or password');
			return false;
		}
		return true;
	};

	const SubmitButton = () => (
		<button
			ref={submitButton}
			type='submit'
			className='button default mt-4'
			disabled={!email || !password}
			onClick={validateInput}>
			Submit
		</button>
	);

	return (
		<div className='auth-inner'>
			<h3>Welcome Back</h3>
			<EmailInput
				email={email}
				error={error}
				onEnterPress={onEnterPress}
				onInput={onEmailInput}
				hasFeedback={false}
			/>
			<PasswordInput
				error={error}
				isPasswordShown={isPasswordShown}
				capsLockWarning={capsLockWarning}
				onEnterPress={onEnterPress}
				onPasswordInput={onPasswordInput}
				togglePassword={togglePassword}
				isSignUp={false}
				ref={passwordInput}
			/>
			<SubmitButton />
			<ForgotPasswordLink />
			<NoAccountLink />
		</div>
	);
}

Login.propTypes = {
	setUserLoggedIn: PropTypes.func.isRequired
};

export default Login;
