import React, { useEffect, useState, useRef, createRef } from 'react';
import { Link } from 'react-router-dom';
import toast from 'react-hot-toast';
import { registerUser } from '../../serverRequests.js';
import { isInputValid } from './inputValidator.js';
import EmailInput from '../emailInput';
import NameInput from './nameInput';
import PasswordInput from '../passwordInput';
import ProtectedRoute from '../../protectedRoute';
import SignUpSuccess from './signupSuccess';
import './signup.css';

const AlreadyRegisteredLink = () => (
	<p className='forgot-password text-right'>
		Already registered?
		<Link to='/login'> Sign in</Link>
	</p>
);

function SignUp() {
	const [email, setEmail] = useState();
	const [password, setPassword] = useState();
	const [firstName, setFirstName] = useState();
	const [lastName, setLastName] = useState();
	const [isCapsWarningShown, setCapsWarningShown] = useState(false);
	const [isPasswordShown, setPasswordShown] = useState(false);
	const [isSignUpSuccess, setSignUpSuccess] = useState(false);
	const [errors, setErrors] = useState({
		firstName: '',
		lastName: '',
		email: '',
		password: ''
	});
	const signUpButton = useRef();
	const lastNameInput = useRef();
	const firstNameInput = createRef();
	const passwordInput = useRef();

	const togglePassword = () => {
		setPasswordShown((isPasswordShown) => !isPasswordShown);
	};

	const capsLockWarning = (e) => {
		if (
			e.getModifierState &&
			e.getModifierState('CapsLock') &&
			!isCapsWarningShown
		) {
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

	const onFirstNameInput = (e) => {
		setFirstName(e.target.value);
	};

	const onLastNameInput = (e) => {
		setLastName(e.target.value);
	};

	useEffect(() => {
		if (email)
			setErrors((prevState) => ({
				...prevState,
				email: ''
			}));
	}, [email]);

	useEffect(() => {
		if (password)
			setErrors((prevState) => ({
				...prevState,
				password: ''
			}));
	}, [password]);

	useEffect(() => {
		if (firstName)
			setErrors((prevState) => ({
				...prevState,
				firstName: ''
			}));
	}, [firstName]);

	useEffect(() => {
		if (lastName)
			setErrors((prevState) => ({
				...prevState,
				lastName: ''
			}));
	}, [lastName]);

	const onSignUpClick = () => {
		if (isInputValid(getUserCredentialsToRegister(), setErrors)) {
			addNewUser();
		}
	};

	const getUserCredentialsToRegister = () => {
		return {
			email: email,
			password: password,
			firstName: firstName,
			lastName: lastName
		};
	};

	const addNewUser = async () => {
		var userData = getUserCredentialsToRegister();
		const isSuccess = await registerUser(userData);
		if (isSuccess) {
			setCapsWarningShown((isCapsWarningShown) => !isCapsWarningShown);
			setSignUpSuccess(true);
		} else
			setErrors((prevState) => ({
				...prevState,
				email: 'Email is invalid'
			}));
	};

	const onEnterPress = (e) => {
		if (e.charCode === 13) {
			switch (e.target.id) {
				case 'floatingEmail':
					firstNameInput.current.focus();
					break;
				case 'floatingFirstName':
					lastNameInput.current.focus();
					break;
				case 'floatingLastName':
					passwordInput.current.focus();
					break;
				case 'floatingPassword':
					signUpButton.current.click();
					break;
				default:
					break;
			}
		}
	};

	const SignUpButton = () => (
		<button
			type='submit'
			className='button default mt-4'
			ref={signUpButton}
			disabled={!email || !password || !firstName || !lastName}
			onClick={onSignUpClick}>
			Sign Up
		</button>
	);

	return isSignUpSuccess ? (
		<SignUpSuccess />
	) : (
		<div className='auth-inner'>
			<h3>Create account</h3>
			<div>
				<EmailInput
					email={email}
					error={errors.email}
					onEnterPress={onEnterPress}
					onInput={onEmailInput}
					hasFeedback
				/>
				<NameInput
					name='First Name'
					error={errors.firstName}
					onEnterPress={onEnterPress}
					onInput={onFirstNameInput}
					ref={firstNameInput}
				/>
				<NameInput
					name='Last Name'
					error={errors.lastName}
					onEnterPress={onEnterPress}
					onInput={onLastNameInput}
					ref={lastNameInput}
				/>
				<PasswordInput
					error={errors.password}
					isPasswordShown={isPasswordShown}
					capsLockWarning={capsLockWarning}
					onEnterPress={onEnterPress}
					onPasswordInput={onPasswordInput}
					togglePassword={togglePassword}
					isSignUp
					ref={passwordInput}
				/>
				<SignUpButton />
				<AlreadyRegisteredLink />
			</div>
		</div>
	);
}

export function WrappedSignup() {
	return (
		<ProtectedRoute>
			<SignUp />
		</ProtectedRoute>
	);
}

export default SignUp;
