import React, { useEffect, useState, useRef, createRef } from 'react';
import { Link } from 'react-router-dom';
import toast from 'react-hot-toast';
import EmailInput from '../emailInput';
import NameInput from './nameInput';
import PasswordInput from '../passwordInput';
import SignUpSuccess from './signupSuccess';
import './signup.css';

const AlreadyRegisteredLink = () => (
	<p className='forgot-password text-right'>
		Already registered?
		<Link to='/login'> Sign in</Link>
	</p>
);

function SignUp() {
	const localStorage = window.localStorage;
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

	const isFirstNameValid = () => {
		const regex = /^[a-zA-ZаЯЁёА-я]+([-]?\s?[a-zA-ZЁёА-я])?$/;
		return regex.test(firstName);
	};

	const isLastNameValid = () => {
		const regex = /^[a-zA-ZаЯЁёА-я]+([-]?\s?[a-zA-ZЁёА-я])?$/;
		return regex.test(lastName);
	};

	const validateName = () => {
		if (!isFirstNameValid()) {
			setErrors((prevState) => ({
				...prevState,
				firstName: 'First name is invalid'
			}));
			return false;
		}
		return true;
	};

	const validateLastName = () => {
		if (!isLastNameValid()) {
			setErrors((prevState) => ({
				...prevState,
				lastName: 'Last name is invalid'
			}));
			return false;
		}
		return true;
	};

	const isCorrectEmailFormat = () => {
		const regex =
			/^(([^<>()[\].,;:\s@"]+(\.[^<>()[\].,;:\s@"]+)*)|(".+"))@(([^<>()[\].,;:\s@"]+\.)+[^<>()[\].,;:\s@"]{2,})$/i;
		return regex.test(email);
	};

	const isEmailBeingUsed = () => {
		const users = JSON.parse(localStorage.getItem('Users')) || [];
		return users.some((user) => user.email === email);
	};

	const validateEmail = () => {
		if (!isCorrectEmailFormat() || isEmailBeingUsed()) {
			setErrors((prevState) => ({
				...prevState,
				email: 'Email is invalid'
			}));
			return false;
		}
		return true;
	};

	const validatePassword = () => {
		if (password.length < 8) {
			setErrors((prevState) => ({
				...prevState,
				password: 'Password is invalid. It must be at least 8 characters'
			}));
			return false;
		}
		return true;
	};

	const onSignUpClick = () => {
		if (
			validateName() &&
			validateLastName() &&
			validateEmail() &&
			validatePassword()
		) {
			registerUser();
			setCapsWarningShown((isCapsWarningShown) => !isCapsWarningShown);
			setSignUpSuccess(true);
		}
	};

	const registerUser = () => {
		const users = JSON.parse(localStorage.getItem('Users')) || [];
		var userData = {
			email: email,
			password: password,
			firstName: firstName,
			lastName: lastName
		};
		users.push(userData);
		localStorage.setItem('Users', JSON.stringify(users));
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
					onInput={onLastNameInput}
					ref={firstNameInput}
				/>
				<NameInput
					name='Last Name'
					error={errors.lastName}
					onEnterPress={onEnterPress}
					onInput={onFirstNameInput}
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

export default SignUp;
