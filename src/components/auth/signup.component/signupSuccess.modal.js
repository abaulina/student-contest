import React from 'react';
import PropTypes from 'prop-types';
import { Link } from 'react-router-dom';

Modal.propTypes = {
	isShowModal: PropTypes.bool.isRequired,
	hideModal: PropTypes.func.isRequired
};

function Modal(props) {
	return (
		props.isShowModal && (
			<div
				className='modal'
				id='addEditModal'
				data-backdrop='static'
				data-keyboard='false'
				tabIndex='-1'
				role='document'>
				<div className='modal-dialog modal-dialog-centered'>
					<div className='modal-content border border-info'>
						<ModalHeader hideModal={props.hideModal} />
						<div className='modal-body'>
							<p>
								Your account has been successfully created. Now you can log in.
							</p>
						</div>
						<ModalFooter hideModal={props.hideModal} />
					</div>
				</div>
			</div>
		)
	);
}

const ModalHeader = (hideModal) => {
	return (
		<div className='modal-header'>
			<h5 className='modal-title'>Thanks for signing up</h5>
			<button
				type='button'
				className='close'
				onClick={hideModal}
				aria-label='Close'
				data-bs-dismiss='modal'>
				<span aria-hidden='true'>&times;</span>
			</button>
		</div>
	);
};

const ModalFooter = (hideModal) => {
	return (
		<div className='modal-footer'>
			<button
				type='button'
				className='btn btn-secondary'
				data-bs-dismiss='modal'
				onClick={hideModal}>
				Close
			</button>
			<button className='button default'>
				<Link className='sign-up' to={'/login'}>
					Log in
				</Link>
			</button>
		</div>
	);
};

export default Modal;
