import React, { useEffect, useRef, useState } from 'react';
import { Link } from 'react-router-dom';
import toast from 'react-hot-toast';
import { useNavigate, useLocation } from 'react-router-dom';
import EmailInput from '../emailInput';
import PasswordInput from '../passwordInput';
import ProtectedRoute from '../../protectedRoute';
import useAuth from '../useAuth';
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

function Login() {
	const [email, setEmail] = useState('');
	const [password, setPassword] = useState('');
	const [isPasswordShown, setPasswordShown] = useState(false);
	const [isCapsWarningShown, setCapsWarningShown] = useState(false);
	const [error, setError] = useState('');
	const submitButton = useRef();
	const passwordInput = useRef();
	const auth = useAuth();
	const navigate = useNavigate();
	const location = useLocation();

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

	const doLogin = async () => {
		await auth.login(getLoginCredentials());
		if (!auth.isAuthenticated) {
			setError('Invalid email or password');
		} else {
			setCapsWarningShown((isCapsWarningShown) => !isCapsWarningShown);
			setError(null);
			if (!location.state || (location.state && !location.state.path))
				navigate('/user');
			else navigate(location.state.path);
		}
	};

	const getLoginCredentials = () => {
		return {
			email: email,
			password: password
		};
	};

	const SubmitButton = () => (
		<button
			ref={submitButton}
			type='submit'
			className='button default mt-4'
			disabled={!email || !password}
			onClick={doLogin}>
			Submit
		</button>
	);

	return (
		<div className='auth-inner' data-testid='loginForm'>
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

export function WrappedLogin() {
	return (
		<ProtectedRoute>
			<Login />
		</ProtectedRoute>
	);
}

export default Login;
