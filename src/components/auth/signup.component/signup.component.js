import React, { useState, useRef } from 'react';
import { Link } from 'react-router-dom';
import toast from 'react-hot-toast';
import { useForm } from 'react-hook-form';
import hidePwdImg from '../../../assets/hide-password.svg';
import Modal from './signupSuccess.modal';
import showPwdImg from '../../../assets/show-password.svg';
import './signup.component.css';

export function SignUp() {
	const localStorage = window.localStorage;
	const [email, setEmail] = useState();
	const [password, setPassword] = useState();
	const [firstName, setFirstName] = useState();
	const [lastName, setLastName] = useState();
	const [isCapsWarningShown, setCapsWarningShown] = useState(false);
	const [isPasswordShown, setPasswordShown] = useState(false);
	const [isShowSuccessModal, setShowSuccessModal] = useState(false);
	const signUpButton = useRef();

	const {
		register,
		handleSubmit,
		reset,
		formState: { errors }
	} = useForm();

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

	const isCorrectEmailFormat = (email) => {
		const regex =
			/^(([^<>()[\].,;:\s@"]+(\.[^<>()[\].,;:\s@"]+)*)|(".+"))@(([^<>()[\].,;:\s@"]+\.)+[^<>()[\].,;:\s@"]{2,})$/i;
		return regex.test(email);
	};

	const isEmailBeingUsed = (email) => {
		const users = JSON.parse(localStorage.getItem('Users')) || [];
		return !users.some((user) => user.email === email);
	};

	const validatePassword = (password) => {
		if (password.length < 8) return false;
		return true;
	};

	const onSignUpClick = () => {
		const users = JSON.parse(localStorage.getItem('Users')) || [];
		var userData = {
			email: email,
			password: password,
			firstName: firstName,
			lastName: lastName
		};
		users.push(userData);
		localStorage.setItem('Users', JSON.stringify(users));
		setCapsWarningShown((isCapsWarningShown) => !isCapsWarningShown);
		reset();
		setShowSuccessModal(true);
	};

	const hideModal = () => {
		setShowSuccessModal(false);
	};

	const onEnterPress = (e) => {
		if (e.charCode === 13) {
			signUpButton.current.click();
		}
	};

	const SignUpButton = () => (
		<button
			type='submit'
			className='button default mt-4'
			ref={signUpButton}
			disabled={errors ? false : true}
			onClick={handleSubmit(onSignUpClick)}>
			Sign Up
		</button>
	);

	const ForgotPasswordLink = () => (
		<p className='forgot-password text-right'>
			Already registered?
			<Link to='/login'> Sign in</Link>
		</p>
	);

	return (
		<div className='auth-inner'>
			<h3>Create account</h3>
			<div>
				<div className='form-floating'>
					<input
						type='text'
						{...register('firstname', {
							required: true
						})}
						className={`form-control ${errors.firstname ? 'is-invalid' : ''}`}
						id='floatingFirstName'
						onChange={onFirstNameInput}
						onKeyPress={onEnterPress}
						placeholder='First name'
						autoFocus
					/>
					<label htmlFor='floatingFirstName'>First Name</label>
					<div className='invalid-feedback'>{errors.firstname?.message}</div>
				</div>
				<div className='form-floating'>
					<input
						type='text'
						{...register('lastname', {
							required: true
						})}
						className={`form-control ${errors.lastname ? 'is-invalid' : ''}`}
						onChange={onLastNameInput}
						onKeyPress={onEnterPress}
						id='floatingLastName'
						placeholder='Last name'
					/>
					<label htmlFor='floatingLastName'>Last name</label>
					<div className='invalid-feedback'>{errors.lastname?.message}</div>
				</div>
				<div className='form-floating'>
					<input
						type='email'
						{...register('email', {
							validate: {
								correctFormat: (value) =>
									isCorrectEmailFormat(value) === true || 'Email is invalid',
								isBeingUsed: (value) =>
									isEmailBeingUsed(value) === true ||
									'This email address is already being used. Please log in'
							}
						})}
						className={`form-control ${errors.email ? 'is-invalid' : ''}`}
						onChange={onEmailInput}
						onKeyPress={onEnterPress}
						id='floatingInput'
						placeholder='name@example.com'
					/>
					<label htmlFor='floatingInput'>Email address</label>
					<div className='invalid-feedback'>{errors.email?.message}</div>
				</div>
				<div className='form-floating d-flex sign-up'>
					<input
						type={isPasswordShown ? 'text' : 'password'}
						{...register('password', {
							validate: (value) =>
								validatePassword(value) === true ||
								'Password must be at least 8 characters'
						})}
						className={`form-control ${errors.password ? 'is-invalid' : ''}`}
						id='floatingPassword'
						placeholder='Password'
						onKeyUp={capsLockWarning}
						onKeyPress={onEnterPress}
						onChange={onPasswordInput}
					/>
					<label htmlFor='floatingPassword'>Password</label>
					<img
						htmlFor='floatingPassword'
						alt=''
						height='24'
						width='24'
						title={isPasswordShown ? 'Hide password' : 'Show password'}
						src={isPasswordShown ? hidePwdImg : showPwdImg}
						onClick={togglePassword}
					/>
					<div className='invalid-feedback'>{errors.password?.message}</div>
				</div>
				<SignUpButton />
				<ForgotPasswordLink />
				<Modal isShowModal={isShowSuccessModal} hideModal={hideModal} />
			</div>
		</div>
	);
}
